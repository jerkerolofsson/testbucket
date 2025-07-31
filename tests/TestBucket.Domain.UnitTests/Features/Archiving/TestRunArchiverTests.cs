using TestBucket.Domain.Features.Archiving;
using TestBucket.Domain.Features.Archiving.Models;
using TestBucket.Domain.Testing.Specifications.TestRuns;
using TestBucket.Domain.UnitTests.Testing.Fakes;

namespace TestBucket.Domain.UnitTests.Features.Archiving;

/// <summary>
/// Unit tests for the <see cref="TestRunArchiver"/> class.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Testing")]
[Feature("Archiving")]
public class TestRunArchiverTests
{
    /// <summary>
    /// Tests that TestRunArchiver.ArchiveProjectRunsAsync archives eligible test runs based on the provided settings.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ArchiveProjectRunsAsync_ShouldArchiveEligibleTestRuns()
    {
        // Arrange
        var fakeRepository = new FakeTestRunRepository();
        var tenantId = "tenant1";
        var projectId = 1L;
        var archivingSettings = new ArchiveSettings
        {
            ArchiveTestRunsAutomatically = true,
            AgeBeforeArchivingTestRuns = TimeSpan.FromDays(30)
        };

        var oldTestRun = new TestRun
        {
            Id = 1,
            TenantId = tenantId,
            Name = "Old Test Run",
            FolderId = projectId,
            Created = DateTimeOffset.UtcNow.AddDays(-40),
            Archived = false,
            Open = false
        };

        var recentTestRun = new TestRun
        {
            Id = 2,
            TenantId = tenantId,
            Name = "Recent Test Run",
            FolderId = projectId,
            Created = DateTimeOffset.UtcNow.AddDays(-10),
            Archived = false,
            Open = false
        };

        await fakeRepository.AddTestRunAsync(oldTestRun);
        await fakeRepository.AddTestRunAsync(recentTestRun);

        // Act
        await TestRunArchiver.ArchiveProjectRunsAsync(fakeRepository, archivingSettings, tenantId, projectId);

        // Assert
        var archivedRuns = await fakeRepository.SearchTestRunsAsync(new[] { new OnlyArchivedTestRuns() }, 0, 10);
        Assert.Single(archivedRuns.Items);
        Assert.Equal(1, archivedRuns.Items.First().Id);
    }

    /// <summary>
    /// Tests that TestRunArchiver.ArchiveProjectRunsAsync does not archive test runs if ArchiveTestRunsAutomatically is false.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ArchiveProjectRunsAsync_ShouldNotArchiveIfAutomaticArchivingIsDisabled()
    {
        // Arrange
        var fakeRepository = new FakeTestRunRepository();
        var tenantId = "tenant1";
        var projectId = 1L;
        var archivingSettings = new ArchiveSettings
        {
            ArchiveTestRunsAutomatically = false,
            AgeBeforeArchivingTestRuns = TimeSpan.FromDays(30)
        };

        var oldTestRun = new TestRun
        {
            Id = 1,
            TenantId = tenantId,
            Name = "Old Test Run",
            FolderId = projectId,
            Created = DateTimeOffset.UtcNow.AddDays(-40),
            Archived = false,
            Open = false
        };

        await fakeRepository.AddTestRunAsync(oldTestRun);

        // Act
        await TestRunArchiver.ArchiveProjectRunsAsync(fakeRepository, archivingSettings, tenantId, projectId);

        // Assert
        var archivedRuns = await fakeRepository.SearchTestRunsAsync(new[] { new OnlyArchivedTestRuns() }, 0, 10);
        Assert.Empty(archivedRuns.Items);
    }

    /// <summary>
    /// Tests that TestRunArchiver.ArchiveProjectRunsAsync does not archive test runs if AgeBeforeArchivingTestRuns is TimeSpan.Zero.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ArchiveProjectRunsAsync_ShouldNotArchiveIfAgeBeforeArchivingIsZero()
    {
        // Arrange
        var fakeRepository = new FakeTestRunRepository();
        var tenantId = "tenant1";
        var projectId = 1L;
        var archivingSettings = new ArchiveSettings
        {
            ArchiveTestRunsAutomatically = true,
            AgeBeforeArchivingTestRuns = TimeSpan.Zero
        };

        var oldTestRun = new TestRun
        {
            Id = 1,
            TenantId = tenantId,
            Name = "Old Test Run",
            FolderId = projectId,
            Created = DateTimeOffset.UtcNow.AddDays(-40),
            Archived = false,
            Open = false
        };

        await fakeRepository.AddTestRunAsync(oldTestRun);

        // Act
        await TestRunArchiver.ArchiveProjectRunsAsync(fakeRepository, archivingSettings, tenantId, projectId);

        // Assert
        var archivedRuns = await fakeRepository.SearchTestRunsAsync(new[] { new OnlyArchivedTestRuns() }, 0, 10);
        Assert.Empty(archivedRuns.Items);
    }

    /// <summary>
    /// Tests that TestRunArchiver.ArchiveProjectRunsAsync does not archive test runs if AgeBeforeArchivingTestRuns is negative.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ArchiveProjectRunsAsync_ShouldNotArchiveIfAgeBeforeArchivingIsNegative()
    {
        // Arrange
        var fakeRepository = new FakeTestRunRepository();
        var tenantId = "tenant1";
        var projectId = 1L;
        var archivingSettings = new ArchiveSettings
        {
            ArchiveTestRunsAutomatically = true,
            AgeBeforeArchivingTestRuns = TimeSpan.FromDays(-1)
        };

        var oldTestRun = new TestRun
        {
            Id = 1,
            TenantId = tenantId,
            Name = "Old Test Run",
            FolderId = projectId,
            Created = DateTimeOffset.UtcNow.AddDays(-40),
            Archived = false,
            Open = false
        };

        await fakeRepository.AddTestRunAsync(oldTestRun);

        // Act
        await TestRunArchiver.ArchiveProjectRunsAsync(fakeRepository, archivingSettings, tenantId, projectId);

        // Assert
        var archivedRuns = await fakeRepository.SearchTestRunsAsync(new[] { new OnlyArchivedTestRuns() }, 0, 10);
        Assert.Empty(archivedRuns.Items);
    }
}
