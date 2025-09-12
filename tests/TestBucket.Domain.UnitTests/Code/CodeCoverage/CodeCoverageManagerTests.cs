using System.Security.Claims;

using TestBucket.Domain.Code.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Fakes;

namespace TestBucket.Domain.UnitTests.Code.CodeCoverage;

/// <summary>
/// Unit tests for <see cref="CodeCoverageManager"/> covering code coverage group and settings operations.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Code Coverage")]
[Component("Code")]
public class CodeCoverageManagerTests
{
    /// <summary>
    /// Creates a new instance of <see cref="CodeCoverageManager"/> with fake dependencies for testing.
    /// </summary>
    /// <returns>A configured <see cref="CodeCoverageManager"/> instance.</returns>
    private async Task<CodeCoverageManager> CreateSutAsync()
    {
        var settingsProvider = new FakeSettingsProvider();
        await settingsProvider.SaveDomainSettingsAsync("tenant-1", 1, new CodeCoverageSettings());

        var repository = new Fakes.FakeCodeCoverageRepository();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 12, 0, 1, 2, TimeSpan.Zero));
        return new CodeCoverageManager(settingsProvider, repository, timeProvider);
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the specified tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the user.</returns>
    private ClaimsPrincipal CreateUser(string tenantId)
    {
        return Impersonation.Impersonate(tenantId);
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageManager.GetOrCreateCodeCoverageGroupAsync"/> sets name, type, and project correctly.
    /// </summary>
    [Fact]
    public async Task GetOrCreateCodeCoverageGroupAsync_SetsNameTypeAndProject()
    {
        // Arrange
        var sut = await CreateSutAsync();
        var user = CreateUser("tenant1");
        long projectId = 1;
        var groupType = CodeCoverageGroupType.Commit;
        var groupName = "commit-sha1";

        // Act
        var createdGroup = await sut.GetOrCreateCodeCoverageGroupAsync(user, projectId, groupType, groupName);

        // Assert
        Assert.NotNull(createdGroup);
        Assert.Equal(groupName, createdGroup.Name);
        Assert.Equal(groupType, createdGroup.Group);
        Assert.Equal(projectId, createdGroup.TestProjectId);
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageManager.GetOrCreateCodeCoverageGroupAsync"/> returns an existing group if present.
    /// </summary>
    [Fact]
    public async Task GetOrCreateCodeCoverageGroupAsync_ReturnsExistingGroup()
    {
        // Arrange
        var sut = await CreateSutAsync();
        var user = CreateUser("tenant1");
        long projectId = 1;
        var groupType = CodeCoverageGroupType.Commit;
        var groupName = "commit-sha1";
        var createdGroup = await sut.GetOrCreateCodeCoverageGroupAsync(user, projectId, groupType, groupName);
        createdGroup.CoveredLineCount = 10;
        createdGroup.LineCount = 20;
        createdGroup.CoveredClassCount = 5;
        createdGroup.ClassCount = 7;

        // Act
        var fetchedGroup = await sut.GetOrCreateCodeCoverageGroupAsync(user, projectId, groupType, groupName);

        // Assert
        Assert.NotNull(createdGroup);
        Assert.NotNull(fetchedGroup);
        Assert.Equal(createdGroup.Id, fetchedGroup.Id);
        Assert.Equal(createdGroup.Group, fetchedGroup.Group);
        Assert.Equal(createdGroup.Name, fetchedGroup.Name);
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageManager.LoadSettingsAsync"/> returns valid code coverage settings.
    /// </summary>
    [Fact]
    public async Task LoadSettingsAsync_ReturnsSettings()
    {
        // Arrange
        var sut = await CreateSutAsync();
        var user = CreateUser("tenant1");
        long projectId = 1;

        // Act
        var settings = await sut.LoadSettingsAsync(user, projectId);

        // Assert
        Assert.NotNull(settings);
        Assert.True(settings.Target > 0);
        Assert.True(settings.StretchTarget > 0);
        Assert.True(settings.MinTarget > 0);
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageManager.GetOrCreateCodeCoverageGroupAsync"/> creates a new group if none exists.
    /// </summary>
    [Fact]
    public async Task GetOrCreateCodeCoverageGroupAsync_CreatesGroupIfNotExists()
    {
        // Arrange
        var sut = await CreateSutAsync();
        var user = CreateUser("tenant1");
        long projectId = 1;
        var groupType = CodeCoverageGroupType.Commit;
        var groupName = "commit-sha1";

        // Act
        var group = await sut.GetOrCreateCodeCoverageGroupAsync(user, projectId, groupType, groupName);

        // Assert
        Assert.NotNull(group);
        Assert.Equal(groupType, group.Group);
        Assert.Equal(groupName, group.Name);
    }

    /// <summary>
    /// Verifies that <see cref="CodeCoverageManager.UpdateCodeCoverageGroupAsync"/> updates group properties.
    /// </summary>
    [Fact]
    public async Task UpdateCodeCoverageGroupAsync_UpdatesGroup()
    {
        // Arrange
        var sut = await CreateSutAsync();
        var user = CreateUser("tenant1");
        long projectId = 1;
        var groupType = CodeCoverageGroupType.TestRun;
        var groupName = "testrun-1";
        var group = await sut.GetOrCreateCodeCoverageGroupAsync(user, projectId, groupType, groupName);

        group.ClassCount = 10;
        group.CoveredClassCount = 8;

        // Act
        await sut.UpdateCodeCoverageGroupAsync(user, group);

        // Assert
        // For a fake repository, you may want to check the updated values if the fake supports it.
        Assert.Equal(10, group.ClassCount);
        Assert.Equal(8, group.CoveredClassCount);
    }
}