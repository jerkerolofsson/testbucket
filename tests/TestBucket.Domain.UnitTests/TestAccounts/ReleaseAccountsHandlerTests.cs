using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using TestBucket.Contracts;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestResources.Specifications;
using TestBucket.Domain.UnitTests.TestAccounts.Fakes;

using Xunit;

namespace TestBucket.Domain.UnitTests.TestAccounts;

/// <summary>
/// Unit tests for the <see cref="ReleaseAccountsHandler"/> class.
/// </summary>
[UnitTest]
[EnrichedTest]
[Component("Test Accounts")]
public class ReleaseAccountsHandlerTests
{
    private readonly ITestAccountRepository _mockRepository;
    private readonly ReleaseAccountsHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseAccountsHandlerTests"/> class.
    /// </summary>
    public ReleaseAccountsHandlerTests()
    {
        _mockRepository = Substitute.For<ITestAccountRepository>();
        _handler = new ReleaseAccountsHandler(_mockRepository);
    }

    /// <summary>
    /// Tests that the <see cref="ReleaseAccountsHandler.Handle"/> method applies the correct filters when searching for accounts.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task Handle_ShouldApplyCorrectFilters()
    {
        // Arrange
        var request = new ReleaseAccountsRequest("owner1", "tenant1");
        _mockRepository
            .SearchAsync(Arg.Any<FilterSpecification<TestAccount>[]>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new PagedResult<TestAccount> { Items = Array.Empty<TestAccount>(), TotalCount = 0 });

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _mockRepository.Received(1).SearchAsync(
            Arg.Is<FilterSpecification<TestAccount>[]>(filters =>
                filters.Length == 3 &&
                filters[0] is FilterByTenant<TestAccount> &&
                filters[1] is FindLockedAccount &&
                filters[2] is FindAccountByLockOwner),
            0, 10);
    }

    /// <summary>
    /// Tests that the <see cref="ReleaseAccountsHandler.Handle"/> method unlocks accounts correctly.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task Handle_ShouldUnlockAccounts()
    {
        // Arrange
        var lockedAccount = new TestAccount
        {
            Name = "TestAccount1",
            Type = "wifi",
            Owner = "owner1",
            Locked = true,
            LockOwner = "owner1",
            LockExpires = DateTime.UtcNow
        };
        var request = new ReleaseAccountsRequest("owner1", "tenant1");
        _mockRepository
            .SearchAsync(Arg.Any<FilterSpecification<TestAccount>[]>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new PagedResult<TestAccount> { Items = new[] { lockedAccount }, TotalCount = 1 });

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(lockedAccount.Locked);
        Assert.Null(lockedAccount.LockOwner);
        Assert.Null(lockedAccount.LockExpires);
        await _mockRepository.Received(1).UpdateAsync(lockedAccount);
    }

    /// <summary>
    /// Tests that the <see cref="ReleaseAccountsHandler.Handle"/> method releases all accounts when there are more accounts than a single page.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task Handle_WithMoreAccountsThanASinglePage_AllAccountsReleased()
    {
        int lockedCount = 25;
        string tenant1 = "tenant-1";
        string lockOwner = "lock-owner123";  

        var repository = new FakeTestAccountRepository();

        // Arrange
        var lockedAccounts = Enumerable.Range(0, lockedCount).Select(i => new TestAccount
        {
            Id = i + 1,
            Name = $"TestAccount{i}",
            Type = "wifi",
            Owner = "owner1",
            Locked = true,
            LockOwner = lockOwner,
            TenantId = tenant1
        }).ToArray();

        foreach(var account in lockedAccounts)
        {
            await repository.AddAsync(account);
        }

        var request = new ReleaseAccountsRequest(lockOwner, tenant1);
        var handler = new ReleaseAccountsHandler(repository);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var accounts = await repository.SearchAsync([], 0, lockedCount + 100);
        Assert.Equal(lockedCount, accounts.Items.Length);
        foreach (var account in accounts.Items)
        {
            Assert.False(account.Locked);
            Assert.Null(account.LockOwner);
            Assert.Null(account.LockExpires);
        }
    }

    /// <summary>
    /// Tests that the <see cref="ReleaseAccountsHandler.Handle"/> method does not modify accounts from other tenants.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task Handle_ShouldNotModifyAccountsFromOtherTenants()
    {
        // Arrange
        string tenant1 = "tenant-1";
        string tenant2 = "tenant-2";
        string lockOwner = "lock-owner123";

        var repository = new FakeTestAccountRepository();

        var tenant1Account = new TestAccount
        {
            Id = 1,
            Name = "Tenant1Account",
            Type = "wifi",
            Owner = "owner1",
            Locked = true,
            LockOwner = lockOwner,
            TenantId = tenant1
        };

        var tenant2Account = new TestAccount
        {
            Id = 2,
            Name = "Tenant2Account",
            Type = "wifi",
            Owner = "owner2",
            Locked = true,
            LockOwner = lockOwner,
            TenantId = tenant2,
            LockExpires = DateTime.UtcNow.AddHours(1)
        };

        await repository.AddAsync(tenant1Account);
        await repository.AddAsync(tenant2Account);

        var request = new ReleaseAccountsRequest(lockOwner, tenant1);
        var handler = new ReleaseAccountsHandler(repository);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var accounts = await repository.SearchAsync(new FilterSpecification<TestAccount>[0], 0, 10);

        // Verify tenant1 account is unlocked
        var updatedTenant1Account = accounts.Items.First(a => a.TenantId == tenant1);
        Assert.False(updatedTenant1Account.Locked);
        Assert.Null(updatedTenant1Account.LockOwner);
        Assert.Null(updatedTenant1Account.LockExpires);

        // Verify tenant2 account is unchanged
        var unchangedTenant2Account = accounts.Items.First(a => a.TenantId == tenant2);
        Assert.True(unchangedTenant2Account.Locked);
        Assert.Equal(lockOwner, unchangedTenant2Account.LockOwner);
        Assert.NotNull(unchangedTenant2Account.LockExpires);
    }

    /// <summary>
    /// Tests that the <see cref="ReleaseAccountsHandler.Handle"/> method does not modify accounts with a different lock owner.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task Handle_ShouldNotModifyAccountsWithDifferentLockOwner()
    {
        // Arrange
        string tenant1 = "tenant-1";
        string lockOwner = "lock-owner123";
        string differentLockOwner = "different-lock-owner";

        var repository = new FakeTestAccountRepository();

        var accountWithDifferentLockOwner = new TestAccount
        {
            Id = 1,
            Name = "AccountWithDifferentLockOwner",
            Type = "wifi",
            Owner = "owner1",
            Locked = true,
            LockOwner = differentLockOwner,
            TenantId = tenant1,
            LockExpires = DateTime.UtcNow.AddHours(1)
        };

        await repository.AddAsync(accountWithDifferentLockOwner);

        var request = new ReleaseAccountsRequest(lockOwner, tenant1);
        var handler = new ReleaseAccountsHandler(repository);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var accounts = await repository.SearchAsync(new FilterSpecification<TestAccount>[0], 0, 10);

        // Verify the account with a different lock owner is unchanged
        var unchangedAccount = accounts.Items.First(a => a.Id == accountWithDifferentLockOwner.Id);
        Assert.True(unchangedAccount.Locked);
        Assert.Equal(differentLockOwner, unchangedAccount.LockOwner);
        Assert.NotNull(unchangedAccount.LockExpires);
    }
}