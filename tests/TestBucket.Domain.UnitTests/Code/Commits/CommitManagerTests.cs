using System.Security.Claims;
using System.Threading.Tasks;
using Mediator;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;
using Xunit;

namespace TestBucket.Domain.UnitTests.Code.Commits;

/// <summary>
/// Exhaustive tests for <see cref="CommitManager"/>.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Code")]
public class CommitManagerTests
{
    private const string TENANT_ID = "tenant-1";
    private const long PROJECT_ID = 42;
    private const long TEAM_ID = 1;

    /// <summary>
    /// Creates a new instance of <see cref="CommitManager"/> and a <see cref="FakeTimeProvider"/> for testing.
    /// </summary>
    /// <returns>A tuple containing the <see cref="CommitManager"/> and <see cref="FakeTimeProvider"/>.</returns>
    private (CommitManager, FakeTimeProvider) CreateSut()
    {
        var repo = new Fakes.FakeCommitRepository();
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 18, 0, 1, 2, TimeSpan.Zero));
        var manager = new CommitManager(repo, mediator, timeProvider);
        return (manager, timeProvider);
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission level, name, and tenant ID.
    /// </summary>
    /// <param name="level">The permission level to assign.</param>
    /// <param name="name">The user name or email. If null, a default is used.</param>
    /// <param name="tenantId">The tenant ID. If null, a default is used.</param>
    /// <returns>A configured <see cref="ClaimsPrincipal"/>.</returns>
    private ClaimsPrincipal CreateUser(PermissionLevel level, string? name = null, string? tenantId = null) => Impersonation.Impersonate(x =>
    {
        x.TenantId = tenantId ?? TENANT_ID;
        x.UserName = x.Email = name ?? "user@nasa.gov";
        x.Add(PermissionEntityType.Architecture, level);
    });

    /// <summary>
    /// Verifies that <see cref="CommitManager.AddCommitAsync"/> adds a commit with metadata.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddCommitAsync_AddsCommitWithMetadata()
    {
        var (sut, timeProvider) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var commit = new Commit
        {
            TeamId = TEAM_ID, TestProjectId = PROJECT_ID,
            Sha = "abc123",
            Reference = "refs/heads/main",
            Message = "Initial commit",
            RepositoryId = 1
        };

        await sut.AddCommitAsync(user, commit);

        var found = await sut.GetCommitByShaAsync(user, "abc123");
        Assert.NotNull(found);
        Assert.Equal("abc123", found.Sha);
        Assert.Equal("Initial commit", found.Message);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.AddCommitAsync"/> throws if the user lacks permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddCommitAsync_ThrowsIfNoPermission()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.Read);
        var commit = new Commit
        {
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
            Sha = "abc123",
            Reference = "refs/heads/main",
            Message = "Initial commit",
            RepositoryId = 1
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.AddCommitAsync(user, commit));
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.AddCommitAsync"/> throws if the user identity is missing.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddCommitAsync_ThrowsIfNoIdentity()
    {
        var (sut, _) = CreateSut();
        var user = new ClaimsPrincipal();
        var commit = new Commit
        {
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
            Sha = "abc123",
            Reference = "refs/heads/main",
            Message = "Initial commit",
            RepositoryId = 1
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.AddCommitAsync(user, commit));
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.UpdateCommitAsync"/> updates a commit.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateCommitAsync_UpdatesCommit()
    {
        var (sut, timeProvider) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var commit = new Commit
        {
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
            Sha = "abc123",
            Reference = "refs/heads/main",
            Message = "Initial commit",
            RepositoryId = 1
        };

        await sut.AddCommitAsync(user, commit);

        commit.Message = "Updated commit message";
        await sut.UpdateCommitAsync(user, commit);

        var updated = await sut.GetCommitByShaAsync(user, "abc123");
        Assert.NotNull(updated);
        Assert.Equal("Updated commit message", updated.Message);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.UpdateCommitAsync"/> throws if the user lacks permission.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateCommitAsync_ThrowsIfNoPermission()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var commit = new Commit
        {
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
            Sha = "abc123",
            Reference = "refs/heads/main",
            Message = "Initial commit",
            RepositoryId = 1
        };

        await sut.AddCommitAsync(user, commit);

        var noWriteUser = CreateUser(PermissionLevel.Read);
        commit.Message = "Should not update";
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.UpdateCommitAsync(noWriteUser, commit));
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.BrowseCommitsAsync"/> returns paged results.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task BrowseCommitsAsync_ReturnsPagedResults()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        for (int i = 0; i < 5; i++)
        {
            await sut.AddCommitAsync(user, new Commit
            {
                TeamId = TEAM_ID,
                TestProjectId = PROJECT_ID,
                Sha = $"sha{i}",
                Reference = "refs/heads/main",
                Message = $"Commit {i}",
                RepositoryId = 1
            });
        }

        var result = await sut.BrowseCommitsAsync(user, PROJECT_ID, 0, 3);
        Assert.Equal(3, result.Items.Length);
        Assert.Equal(5, result.TotalCount);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.SearchCommitsAsync"/> filters commits by text.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task SearchCommitsAsync_FiltersByText()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        await sut.AddCommitAsync(user, new Commit
        {
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
            Sha = "sha1",
            Reference = "refs/heads/main",
            Message = "Fix bug",
            RepositoryId = 1
        });
        await sut.AddCommitAsync(user, new Commit
        {
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
            Sha = "sha2",
            Reference = "refs/heads/main",
            Message = "Add feature",
            RepositoryId = 1
        });

        var result = await sut.SearchCommitsAsync(user, PROJECT_ID, "feature", 0, 10);
        Assert.Single(result.Items);
        Assert.Equal("sha2", result.Items[0].Sha);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.GetCommitByShaAsync"/> returns null if the commit is not found.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetCommitByShaAsync_ReturnsNullIfNotFound()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        var result = await sut.GetCommitByShaAsync(user, "notfound");
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.AddRepositoryAsync"/> adds a repository.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddRepositoryAsync_AddsRepository()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var repo = new Repository
        {
            Url = "https://github.com/test/repo",
            ExternalSystemId = 1,
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
        };

        await sut.AddRepositoryAsync(user, repo);

        var found = await sut.GetRepoByExternalSystemAsync(user, 1);
        Assert.NotNull(found);
        Assert.Equal("https://github.com/test/repo", found.Url);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.UpdateRepositoryAsync"/> updates a repository.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateRepositoryAsync_UpdatesRepository()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);
        var repo = new Repository
        {
            Url = "https://github.com/test/repo",
            ExternalSystemId = 1,
            TeamId = TEAM_ID,
            TestProjectId = PROJECT_ID,
        };

        await sut.AddRepositoryAsync(user, repo);

        repo.Url = "https://github.com/test/updated";
        await sut.UpdateRepositoryAsync(user, repo);

        var updated = await sut.GetRepoByExternalSystemAsync(user, 1);
        Assert.NotNull(updated);
        Assert.Equal("https://github.com/test/updated", updated.Url);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.GetRepoByExternalSystemAsync"/> returns null if the repository is not found.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task GetRepoByExternalSystemAsync_ReturnsNullIfNotFound()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        var result = await sut.GetRepoByExternalSystemAsync(user, 999);
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.AddCommitAsync"/> throws if the commit is null.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddCommitAsync_ThrowsIfNull()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.AddCommitAsync(user, null!));
    }

    /// <summary>
    /// Verifies that <see cref="CommitManager.UpdateCommitAsync"/> throws if the commit is null.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateCommitAsync_ThrowsIfNull()
    {
        var (sut, _) = CreateSut();
        var user = CreateUser(PermissionLevel.ReadWrite);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.UpdateCommitAsync(user, null!));
    }
}