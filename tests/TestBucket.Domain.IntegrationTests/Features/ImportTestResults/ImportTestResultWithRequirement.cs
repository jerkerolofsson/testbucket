
using Mediator;

using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Formats;

namespace TestBucket.Domain.IntegrationTests.Features.ImportTestResults
{
    /// <summary>
    /// TEsts related to importing test results
    /// </summary>
    /// <param name="Fixture"></param>
    [Feature("Import Test Results")]
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Testing")]
    public class ImportTestResultWithRequirement(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a link is created between a test and a requirement when the test result has the CoveredRequirement trait
        /// when the requirement matches the CoveredRequirement from "external ID"
        /// 
        /// # Steps
        /// 1. Create a requirement with an external ID
        /// 2. Create a new run
        /// 3. Import xUnit test results where a test result defines a requirement with a matching external ID
        /// 4. Get links for the requirement.Verify that the test case is linked
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportTestResults_WithCoveredRequirementMatchingExternalId_LinkCreated()
        {
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirement = new Requirement { ExternalId = "REQ001", Description = "Hello", Name = "Requirement 1" };
            await Fixture.Requirements.AddRequirementToNewSpecificationAsync(requirement);

            // Add a category field
            await fieldDefinitionManager.AddAsync(principal, FieldDefinitionTemplates.TestCategory);

            // Create a run
            var runManager = Fixture.Services.GetRequiredService<ITestRunManager>();
            var run = new TestRun() { TestProjectId = Fixture.ProjectId, Name = "Run " + Guid.NewGuid().ToString(), TeamId = Fixture.TeamId };
            await runManager.AddTestRunAsync(principal, run);

            // Import results
            var text = await File.ReadAllTextAsync("./TestData/xunit.xml", TestContext.Current.CancellationToken);
            var textImporter = Fixture.Services.GetRequiredService<ITextTestResultsImporter>();
            await textImporter.ImportTextAsync(principal, Fixture.TeamId, Fixture.ProjectId, Formats.TestResultFormat.xUnitXml, text, new ImportHandlingOptions());

            byte[] zipBytes = File.ReadAllBytes("TestData/xunit.zip");
            await mediator.Publish(new JobArtifactDownloaded(principal, run.Id, "**/*.xml", null, zipBytes), TestContext.Current.CancellationToken);

            // Assert
            var links = await requirementManager.GetLinksForRequirementAsync(principal, requirement);
            Assert.Single(links);
        }
    }
}
