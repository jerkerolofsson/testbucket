
using Mediator;
using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Testing.TestRuns;

namespace TestBucket.Domain.IntegrationTests.Features.Automation.Artifacts
{
    [Feature("Import Test Results")]
    [IntegrationTest]
    [EnrichedTest]
    public class JobArtifactDownloadedTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that when a valid test-result job artifact is downloaded from a pipeline, the artifact is imported as test results.

            This is all triggered from the notification: JobArtifactDownloaded.
            The files in the artifact-zip matching the glob-pattern will be added as attachments to the test run.
            
            When the attachment is added another notification is triggered: FileResourceAddedNotification, which will import any
            attachments that are valid.

            This test case injects the JobArtifactDownloaded notification and verifies that test results are added to the run.

            """)]
        public async Task DownloadArtifactFromExternal_WithValidTestResultFile_ResultsAreImported()
        {
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            var principal = Fixture.App.SiteAdministrator;

            var runManager = Fixture.Services.GetRequiredService<ITestRunManager>();
            var run = new TestRun() { TestProjectId = Fixture.ProjectId, Name = "Run " + Guid.NewGuid().ToString(), TeamId = Fixture.TeamId };
            await runManager.AddTestRunAsync(principal, run);

            byte[] zipBytes = File.ReadAllBytes("TestData/xunit.zip");
            await mediator.Publish(new JobArtifactDownloaded(principal, run.Id, "**/*.xml", zipBytes), TestContext.Current.CancellationToken);

            // Get tests from the run
            var result = await runManager.SearchTestCaseRunsAsync(principal, new SearchTestCaseRunQuery { TestRunId = run.Id, Offset = 0, Count = 1 });
            Assert.NotEmpty(result.Items);  
        }
    }
}
