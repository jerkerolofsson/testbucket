using Mediator;
using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Features.ImportTestResults
{
    /// <summary>
    /// Tests releated to importing test results
    /// </summary>
    /// <param name="Fixture"></param>
    [Component("Testing")]
    [Feature("Import Test Results")]
    [IntegrationTest]
    [EnrichedTest]
    public class ImportTestResultsTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that traits are correctly imported as fields
        /// 
        /// # Steps
        /// 1. Create a new run
        /// 2. Import xUnit test results from a job artifact zip file
        /// 3. Verify that the results(TestCaseRun) are tagged with the correct test category: "Integration"
        /// 4. Verify that the test case are tagged with the same category
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportTestResults_FromArtifactZipWithCustomFieldAdded_FieldAddedToTestCaseAndTestCaseRun()
        {
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var principal = Fixture.App.SiteAdministrator;

            // Add a category field
            await fieldDefinitionManager.AddAsync(principal, FieldDefinitionTemplates.TestCategory);

            // Create a run
            var runManager = Fixture.Services.GetRequiredService<ITestRunManager>();
            var run = new TestRun() { TestProjectId = Fixture.ProjectId, Name = "Run " + Guid.NewGuid().ToString(), TeamId = Fixture.TeamId, Open = false };
            await runManager.AddTestRunAsync(principal, run);

            // Import results
            byte[] zipBytes = File.ReadAllBytes("TestData/xunit.zip");
            await mediator.Publish(new JobArtifactDownloaded(principal, run.TestProjectId.Value, run.Id, "**/*.xml", null, zipBytes), TestContext.Current.CancellationToken);

            // Assert

            // Get tests from the run
            var result = await runManager.SearchTestCaseRunsAsync(principal, new SearchTestCaseRunQuery { TestRunId = run.Id, Offset = 0, Count = 1 });
            Assert.NotEmpty(result.Items);
            foreach(var item in result.Items)
            {
                var testCaseFields = await fieldManager.GetTestCaseFieldsAsync(principal, item.TestCaseId, []);
                var testCaseRunFields = await fieldManager.GetTestCaseRunFieldsAsync(principal, run.Id, item.Id, []);

                var testCaseCategoryField = testCaseFields.Where(x => x.FieldDefinition?.TraitType == TraitType.TestCategory).FirstOrDefault();
                var testCaseRunCategoryField = testCaseRunFields.Where(x => x.FieldDefinition?.TraitType == TraitType.TestCategory).FirstOrDefault();
                Assert.NotNull(testCaseCategoryField);
                Assert.NotNull(testCaseRunCategoryField);
                Assert.Equal("Integration", testCaseCategoryField.StringValue);
                Assert.Equal("Integration", testCaseRunCategoryField.StringValue);
            }
        }
    }
}
