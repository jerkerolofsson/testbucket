using TestBucket.Domain.Features.Archiving.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestRuns;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.Features.Archiving;
internal class TestRunArchiver
{
    public static async Task ArchiveProjectRunsAsync(ITestRunRepository testRepo, ArchiveSettings? archivingSettings, string tenantId, long projectId)
    {
        if (archivingSettings?.ArchiveTestRunsAutomatically == true)
        {
            var age = archivingSettings.AgeBeforeArchivingTestRuns;
            if (age > TimeSpan.Zero)
            {
                await ArchiveRunsAsync(testRepo, archivingSettings, tenantId, projectId);
            }
        }
    }

    private static async Task ArchiveRunsAsync(ITestRunRepository testRepo, ArchiveSettings archivingSettings, string tenantId, long projectId)
    {
        var until = DateTimeOffset.UtcNow.Subtract(archivingSettings.AgeBeforeArchivingTestRuns);
        var from = until.AddYears(-1);

        FilterSpecification<TestRun>[] filters =
            [
                new OnlyClosedTestRuns(),
                new ExcludeArchivedTestRuns(),
                new FilterByCreated<TestRun>(from, until),
                new FilterByTenant<TestRun>(tenantId),
                new FilterByProject<TestRun>(projectId)
            ];

        var result = await testRepo.SearchTestRunsAsync(filters, 0, 20);
        foreach (var testRun in result.Items)
        {
            testRun.Archived = true;
            await testRepo.UpdateTestRunAsync(testRun);
        }
    }
}
