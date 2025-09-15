using System;
using System.Security.Claims;
using System.Threading.Tasks;

using TestBucket.Domain;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Comments.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.UnitTests.Comments.Fakes;

using Xunit;

namespace TestBucket.Domain.UnitTests.Comments;

/// <summary>
/// Unit tests for the <see cref="CommentsManager"/> class, covering comment operations and permission checks.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[Component("Comments")]
public class CommentsManagerTests
{
    /// <summary>
    /// Creates a valid <see cref="ClaimsPrincipal"/> for testing with specified tenant and user name.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="name">The user name (optional, defaults to "user1").</param>
    /// <returns>A configured <see cref="ClaimsPrincipal"/>.</returns>
    public ClaimsPrincipal GetValidUser(string tenantId, string name = "user1")
    {
        return Impersonation.Impersonate(configure =>
        {
            configure.TenantId = tenantId;
            configure.UserName = name!;
            configure.Email = name!;
            configure.Add(Domain.Identity.Permissions.PermissionEntityType.Requirement, Domain.Identity.Permissions.PermissionLevel.All);
        });
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.DeleteCommentAsync"/> deletes a comment and checks permissions.
    /// </summary>
    [Fact]
    public async Task DeleteCommentAsync_DeletesCommentAndChecksPermissions()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();
        await manager.AddCommentAsync(principal, comment);

        // Act
        await manager.DeleteCommentAsync(principal, comment);

        // Assert
        Assert.Empty(repo.Comments);
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.DeleteCommentAsync"/> throws if the user has no permission.
    /// </summary>
    [Fact]
    public async Task DeleteCommentAsync_ThrowsIfNoPermission()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();
        await manager.AddCommentAsync(principal, comment);

        // Remove permission
        var noPermissionPrincipal = Impersonation.Impersonate(cfg =>
        {
            cfg.TenantId = "tenant-1";
            cfg.UserName = "user1";
            cfg.Email = "user1";
            // No permissions added
        });

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.DeleteCommentAsync(noPermissionPrincipal, comment));
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.UpdateCommentAsync"/> throws if the tenant does not match.
    /// </summary>
    [Fact]
    public async Task UpdateCommentAsync_ThrowsIfTenantMismatch()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();
        await manager.AddCommentAsync(principal, comment);

        var otherTenantPrincipal = GetValidUser("tenant-2", "user2");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.UpdateCommentAsync(otherTenantPrincipal, comment));
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.DeleteCommentAsync"/> throws if the tenant does not match.
    /// </summary>
    [Fact]
    public async Task DeleteCommentAsync_ThrowsIfTenantMismatch()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();
        await manager.AddCommentAsync(principal, comment);

        var otherTenantPrincipal = GetValidUser("tenant-2", "user2");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.DeleteCommentAsync(otherTenantPrincipal, comment));
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.AddCommentAsync"/> sets timestamps and user information.
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_SetsTimestampAndUser()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();

        // Act
        await manager.AddCommentAsync(principal, comment);

        // Assert
        Assert.Equal(fakeNow, comment.Created);
        Assert.Equal(fakeNow, comment.Modified);
        Assert.Equal("user1", comment.CreatedBy);
        Assert.Equal("user1", comment.ModifiedBy);
        Assert.Equal("tenant-1", comment.TenantId);
        Assert.Single(repo.Comments);
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.UpdateCommentAsync"/> sets the modified timestamp and user.
    /// </summary>
    [Fact]
    public async Task UpdateCommentAsync_SetsModifiedTimestampAndUser()
    {
        // Arrange
        var initialTime = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var updateTime = new DateTimeOffset(2025, 9, 15, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(initialTime);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();
        await manager.AddCommentAsync(principal, comment);

        // Act
        timeProvider.SetDateTime(updateTime);
        var updatePrincipal = GetValidUser("tenant-1", "user2");
        await manager.UpdateCommentAsync(updatePrincipal, comment);

        // Assert
        Assert.Equal(initialTime, comment.Created);
        Assert.Equal(updateTime, comment.Modified);
        Assert.Equal("user1", comment.CreatedBy);
        Assert.Equal("user2", comment.ModifiedBy);
        Assert.Equal("tenant-1", comment.TenantId);
        Assert.Single(repo.Comments);
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.AddCommentAsync"/> throws if the identity name is null.
    /// </summary>
    [Fact]
    public async Task AddCommentAsync_ThrowsIfIdentityNameIsNull()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = new ClaimsPrincipal();
        var comment = new Comment();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.AddCommentAsync(principal, comment));
    }

    /// <summary>
    /// Verifies that <see cref="CommentsManager.UpdateCommentAsync"/> throws if the identity name is null.
    /// </summary>
    [Fact]
    public async Task UpdateCommentAsync_ThrowsIfIdentityNameIsNull()
    {
        // Arrange
        var fakeNow = new DateTimeOffset(2025, 9, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fakeNow);
        var repo = new FakeCommentsRepository();
        var manager = new CommentsManager(repo, timeProvider);

        var principal = GetValidUser("tenant-1", "user1");
        var comment = new Comment();
        await manager.AddCommentAsync(principal, comment);

        var principalWithoutIdentity = new ClaimsPrincipal();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            manager.UpdateCommentAsync(principalWithoutIdentity, comment));
    }
}