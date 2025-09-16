using Mediator;

using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.IntegrationTests.Features.Automation.Pipelines
{
    /// <summary>
    /// Tests a related to job artifacts (from CI builds) being imported as test results if their format is supported and
    /// the files are valid.
    /// </summary>
    /// <param name="Fixture"></param>
    [Feature("Import Test Results")]
    [Component("Automation")]
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    public class JobArtifactDownloadedTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that when a valid test-result job artifact is downloaded from a pipeline, the artifact is imported as test results.
        /// 
        /// This is all triggered from the notification: JobArtifactDownloaded.
        /// The files in the artifact-zip matching the glob-pattern will be added as attachments to the test run.
        /// 
        /// When the attachment is added another notification is triggered: FileResourceAddedNotification, which will import any
        /// attachments that are valid.
        /// 
        /// This test case injects the JobArtifactDownloaded notification and verifies that test results are added to the run.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DownloadArtifactFromExternal_WithValidTestResultFile_ResultsAreImported()
        {
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            var principal = Fixture.App.SiteAdministrator;

            var runManager = Fixture.Services.GetRequiredService<ITestRunManager>();
            var run = new TestRun() { TestProjectId = Fixture.ProjectId, Name = "Run " + Guid.NewGuid().ToString(), TeamId = Fixture.TeamId };
            await runManager.AddTestRunAsync(principal, run);

            byte[] zipBytes = File.ReadAllBytes("TestData/xunit.zip");
            await mediator.Publish(new JobArtifactDownloaded(principal, run.TestProjectId.Value, run.Id, "**/*.xml", null, zipBytes), TestContext.Current.CancellationToken);

            // Get tests from the run
            var result = await runManager.SearchTestCaseRunsAsync(principal, new SearchTestCaseRunQuery { TestRunId = run.Id, Offset = 0, Count = 1 });
            Assert.NotEmpty(result.Items);  
        }
    }
}
