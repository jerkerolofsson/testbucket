using System.Security.Claims;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.UnitTests.TestAccounts.Fakes;

namespace TestBucket.Domain.UnitTests.TestAccounts;

/// <summary>
/// Unit tests for the <see cref="TestAccountManager"/> class, focusing on CRUD operations and permission handling.
/// </summary>
[UnitTest]
[EnrichedTest]
[Component("Test Accounts")]
public class TestAccountManagerTests
{
    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission level.
    /// </summary>
    /// <param name="level">The permission level to assign to the principal.</param>
    /// <param name="tenantId">The tenant ID to associate with the principal. Defaults to "tenant-1" if not provided.</param>
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
    /// Tests that <see cref="TestAccountManager.BrowseAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user lacks permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task BrowseAsync_WithMissingPermissions_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All);
        var user2 = CreatePrincipal(PermissionLevel.None);
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.BrowseAsync(user2, 0, 100));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.SearchAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user lacks permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task SearchAsync_WithMissingPermissions_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All);
        var user2 = CreatePrincipal(PermissionLevel.None);
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.SearchAsync(user2, "Name", 0, 100));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.GetAccountByIdAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user lacks permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task GetAccountByIdAsync_WithMissingPermissions_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All);
        var user2 = CreatePrincipal(PermissionLevel.None);
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.GetAccountByIdAsync(user2, account.Id));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.UpdateAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user lacks permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateAsync_WithMissingPermissions_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All);
        var user2 = CreatePrincipal(PermissionLevel.Read);
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.DeleteAsync(user2, account));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.DeleteAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user lacks permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteAsync_WithMissingPermissions_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All);
        var user2 = CreatePrincipal(PermissionLevel.ReadWrite);
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.DeleteAsync(user2, account));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.DeleteAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user belongs to a different tenant.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteAsync_WithUserFromOtherTenant_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All, "tenant-1");
        var user2 = CreatePrincipal(PermissionLevel.All, "tenant-2");
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.DeleteAsync(user2, account));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.UpdateAsync"/> throws an <see cref="UnauthorizedAccessException"/> when the user belongs to a different tenant.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateAsync_WithUserFromOtherTenant_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user1 = CreatePrincipal(PermissionLevel.All, "tenant-1");
        var user2 = CreatePrincipal(PermissionLevel.All, "tenant-2");
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(user1, account);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await manager.UpdateAsync(user2, account));
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.AddAsync"/> adds an account successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddAsync_ShouldAddAccount()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.Write);
        var account = new TestAccount { Id = 1, Name = "Test Account", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        // Act
        await manager.AddAsync(principal, account);

        // Assert
        var addedAccount = await repository.GetAccountByIdAsync("tenant-1", 1);
        Assert.NotNull(addedAccount);
        Assert.Equal("Test Account", addedAccount!.Name);

        Assert.Equal("tenant-1", addedAccount.TenantId);
        Assert.Equal(principal.Identity!.Name, addedAccount.CreatedBy);
        Assert.Equal(principal.Identity!.Name, addedAccount.ModifiedBy);
        Assert.Equal(timeProvider.GetUtcNow(), addedAccount.Created);
        Assert.Equal(timeProvider.GetUtcNow(), addedAccount.Modified);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.UpdateAsync"/> updates an account successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateAsync_ShouldUpdateAccount()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.Write);
        var account = new TestAccount { Id = 1, Name = "Old Name", Owner = "owner1", Type = "wifi" };
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(principal, account);

        account.Name = "Updated Name";
        account.ModifiedBy = "";

        timeProvider.SetDateTime(new DateTimeOffset(2025, 10, 10, 2, 3, 4, TimeSpan.Zero));

        // Act
        await manager.UpdateAsync(principal, account);

        // Assert
        var updatedAccount = await repository.GetAccountByIdAsync("tenant-1", 1);
        Assert.NotNull(updatedAccount);
        Assert.Equal("Updated Name", updatedAccount!.Name);
        Assert.Equal(principal.Identity!.Name, updatedAccount.ModifiedBy);
        Assert.Equal(timeProvider.GetUtcNow(), updatedAccount.Modified);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.BrowseAsync"/> returns a single page of accounts when there is a large dataset.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task BrowseAsync_WithLotsOfData_ReturnsASinglePage()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.Read);
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        for (int i = 0; i < 11; i++)
        {
            await manager.AddAsync(admin, new TestAccount { Id = 1 + i, Name = $"Account {i + 1}", Owner = "owner1", Type = "wifi" });
        }

        // Act
        var result = await manager.BrowseAsync(user, 0, 10);

        // Assert
        Assert.Equal(10, result.Items.Length);
        Assert.Equal(11, result.TotalCount);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.BrowseAsync"/> returns the correct accounts.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task BrowseAsync_ShouldReturnAccounts()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.Read);
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(admin, new TestAccount { Id = 1, Name = "Account 1", Owner = "owner1", Type = "wifi" });
        await manager.AddAsync(admin, new TestAccount { Id = 2, Name = "Account 2", Owner = "owner1", Type = "wifi" });

        // Act
        var result = await manager.BrowseAsync(user, 0, 10);

        // Assert
        Assert.Equal(2, result.Items.Length);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.SearchAsync"/> matches accounts by type.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task SearchAsync_ShouldMatchType()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.Read);
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(admin, new TestAccount { Id = 1, Name = "X", Owner = "owner1", Type = "wifi" });
        await manager.AddAsync(admin, new TestAccount { Id = 2, Name = "Y", Owner = "owner1", Type = "spotify" });

        // Act
        var result = await manager.SearchAsync(user, "wifi", 0, 10);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("X", result.Items.First().Name);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.SearchAsync"/> filters accounts correctly.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task SearchAsync_ShouldFilterAccounts()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.Read);

        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        await manager.AddAsync(admin, new TestAccount { Id = 1, Name = "Match", Owner = "owner1", Type = "wifi" });
        await manager.AddAsync(admin, new TestAccount { Id = 2, Name = "ZZZZZZ", Owner = "owner1", Type = "wifi" });

        // Act
        var result = await manager.SearchAsync(user, "Match", 0, 10);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Match", result.Items.First().Name);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.DeleteAsync"/> removes an account successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteAsync_ShouldRemoveAccount()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All);
        var user = CreatePrincipal(PermissionLevel.Read | PermissionLevel.Delete);
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        var account = new TestAccount { Id = 1, Name = "To Delete", Owner = "owner1", Type = "wifi" };
        await manager.AddAsync(admin, account);

        // Act
        await manager.DeleteAsync(user, account);

        // Assert
        var deletedAccount = await repository.GetAccountByIdAsync("tenant-1", 1);
        Assert.Null(deletedAccount);
    }

    /// <summary>
    /// Tests that <see cref="TestAccountManager.GetAccountByIdAsync"/> retrieves an account successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetAccountByIdAsync_ShouldReturnAccount()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.Read);
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 9, 1, 2, 3, TimeSpan.Zero));
        var repository = new FakeTestAccountRepository();
        var manager = new TestAccountManager(timeProvider, repository);

        var account = new TestAccount { Id = 1, Name = "Existing Account", Owner = "owner1", Type = "wifi" };
        await repository.AddAsync(account);

        // Act
        var result = await manager.GetAccountByIdAsync(principal, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Existing Account", result!.Name);
    }
}