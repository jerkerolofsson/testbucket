using System.Security.Claims;

using Mediator;

using NSubstitute;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.UnitTests.TestAccounts;

/// <summary>
/// Unit tests for <see cref="TestAccountDependencyAllocator"/>.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Test Accounts")]
public class TestAccountDependencyAllocatorTests
{
    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission level.
    /// </summary>
    /// <param name="level">The permission level to assign to the principal.</param>
    /// <param name="tenantId">The tenant ID to assign to the principal. Defaults to "tenant-1" if not provided.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
    private ClaimsPrincipal CreatePrincipal(PermissionLevel level, string? tenantId = null)
    {
        return Impersonation.Impersonate(builder =>
        {
            builder.TenantId = tenantId ?? "tenant-1";
            builder.UserName = "user1@testbucket.io";
            builder.Add(PermissionEntityType.TestAccount, level);
        });
    }

    /// <summary>
    /// Creates an instance of <see cref="TestAccountDependencyAllocator"/> for testing.
    /// </summary>
    /// <returns>A new instance of <see cref="TestAccountDependencyAllocator"/>.</returns>
    internal TestAccountDependencyAllocator CreateSut()
    {
        var repo = new Fakes.FakeTestAccountRepository();
        var manager = new TestAccountManager(TimeProvider.System, repo);
        return new TestAccountDependencyAllocator(manager, repo, Substitute.For<IMediator>());
    }
    
    internal async Task<TestAccountDependencyAllocator> CreateSutAsync(params TestAccount[] accounts)
    {
        var repo = new Fakes.FakeTestAccountRepository();
        var manager = new TestAccountManager(TimeProvider.System, repo);

        foreach(var account in accounts)
        { 
            await manager.AddAsync(CreatePrincipal(PermissionLevel.All), account);
        }

        return new TestAccountDependencyAllocator(manager, repo, Substitute.For<IMediator>());
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> returns a <see cref="TestAccountBag"/> when valid inputs are provided.
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldReturnTestAccountBag_WhenValidInputsProvided()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var context = new TestExecutionContext
        {
            Guid = Guid.NewGuid().ToString(),
            TestRunId = 123,
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Act
        var result = await sut.CollectDependenciesAsync(principal, context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Accounts);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the principal has no permissions.
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldThrowUnauthorizedAccessException_WhenPrincipalHasNoPermissions()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal(PermissionLevel.None);
        var context = new TestExecutionContext
        {
            Guid = Guid.NewGuid().ToString(),
            TestRunId = 123,
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1),
            Dependencies = new List<TestCaseDependency>
            {
                new TestCaseDependency { AccountType = "email" }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.CollectDependenciesAsync(principal, context, CancellationToken.None));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> throws an <see cref="ArgumentNullException"/> when the context is null.
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldThrowArgumentNullException_WhenContextIsNull()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CollectDependenciesAsync(principal, null!, CancellationToken.None));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> throws an <see cref="ArgumentNullException"/> when the principal is null.
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldThrowArgumentNullException_WhenPrincipalIsNull()
    {
        // Arrange
        var sut = CreateSut();
        var context = new TestExecutionContext
        {
            TestRunId = 123,
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CollectDependenciesAsync(null!, context, CancellationToken.None));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> returns an empty <see cref="TestAccountBag"/> when no dependencies are required.
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldReturnEmptyBag_WhenNoDependenciesAreRequired()
    {
        // Arrange
        var sut = await CreateSutAsync([new TestAccount { Name = "a", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true }]);

        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var context = new TestExecutionContext
        {
            TestRunId = 123,
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1),
            Dependencies = new List<TestCaseDependency>()
        };

        // Act
        var result = await sut.CollectDependenciesAsync(principal, context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Accounts);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> allocates accounts when dependencies are specified.
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldAllocateAccounts_WhenDependenciesAreSpecified()
    {
        // Arrange
        var sut = await CreateSutAsync([new TestAccount { Name = "email1", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true }]);

        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var context = new TestExecutionContext
        {
            TestRunId = 123,
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1),
            Dependencies = new List<TestCaseDependency>
            {
                new TestCaseDependency { AccountType = "email" }
            }
        };

        // Act
        var result = await sut.CollectDependenciesAsync(principal, context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Accounts);
        Assert.Equal("email", result.Accounts[0].Type);
        Assert.Equal("email1", result.Accounts[0].Name);
    }


    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> allocates an account which is enabled
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldAllocateEnabledAccount_WhenThereAreDisabledAccounts()
    {
        // Arrange
        var sut = await CreateSutAsync([
            new TestAccount { Name = "not1", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = false },
            new TestAccount { Name = "use1", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true },
            new TestAccount { Name = "not2", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = false },
        ]);

        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var context = new TestExecutionContext
        {
            TestRunId = 123,
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1),
            Dependencies = new List<TestCaseDependency>
            {
                new TestCaseDependency { AccountType = "email" }
            }
        };

        // Act
        var result = await sut.CollectDependenciesAsync(principal, context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Accounts);
        Assert.Equal("use1", result.Accounts[0].Name);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> allocates accounts of the correct type when there are multiple accounts available
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldAllocateAccountsOfCorrectType_WhenThereAreManyWithDifferentTypes()
    {
        // Arrange
        var sut = await CreateSutAsync([
            new TestAccount { Name = "not1", Owner = "a", Type = "other", TenantId = "tenant-1", Enabled = true },
            new TestAccount { Name = "use1", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true },
            new TestAccount { Name = "not2", Owner = "a", Type = "other", TenantId = "tenant-1", Enabled = true },
            new TestAccount { Name = "not3", Owner = "a", Type = "other", TenantId = "tenant-1", Enabled = true },
        ]);

        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var context = new TestExecutionContext
        {
            TestRunId = 123,
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1),
            Dependencies = new List<TestCaseDependency>
            {
                new TestCaseDependency { AccountType = "email" }
            }
        };

        // Act
        var result = await sut.CollectDependenciesAsync(principal, context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Accounts);
        Assert.Equal("email", result.Accounts[0].Type);
        Assert.Equal("use1", result.Accounts[0].Name);
    }


    /// <summary>
    /// Tests that <see cref="TestAccountDependencyAllocator.CollectDependenciesAsync"/> allocates two unique accounts if two accounts of the same type are requested
    /// </summary>
    [Fact]
    public async Task CollectDependenciesAsync_ShouldAllocateUniqueAccounts_WhenTwoOfTheSameTypeAreRequested()
    {
        // Arrange
        var sut = await CreateSutAsync([
            new TestAccount { Name = "email1", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true },
            new TestAccount { Name = "email2", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true },
            new TestAccount { Name = "email3", Owner = "a", Type = "email", TenantId = "tenant-1", Enabled = true },
        ]);

        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var context = new TestExecutionContext
        {
            TestRunId = 123,
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 1,
            ResourceExpiry = DateTimeOffset.UtcNow.AddHours(1),
            Dependencies = new List<TestCaseDependency>
            {
                new TestCaseDependency { AccountType = "email" },
                new TestCaseDependency { AccountType = "email" },
            }
        };

        // Act
        var result = await sut.CollectDependenciesAsync(principal, context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Accounts.Count);
        Assert.True(result.Accounts[0].Name != result.Accounts[1].Name);
    }
}