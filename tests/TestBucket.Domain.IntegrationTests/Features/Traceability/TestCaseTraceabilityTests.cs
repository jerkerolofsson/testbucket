using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Features.Traceability.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.IntegrationTests.Features.Traceability
{
    /// <summary>
    /// Tests related to tracing
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Traceability")]
    public class TestCaseTraceabilityTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that when collecting a trace for a *testcase* with a "test case linked to a requirement" the **requirement**
        /// is returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("requirement-traceability")]
        public async Task GetTestCaseTrace_WithRequirementLink_LinkIncluded()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var testSuiteManager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();
            var testCaseManager = scope.ServiceProvider.GetRequiredService<ITestCaseManager>();
            var principal = Fixture.App.SiteAdministrator;
            var milestoneValue = "1.0";

            // Add requirement
            var requirement = new Requirement { Name = Guid.NewGuid().ToString() };
            await Fixture.Requirements.AddRequirementToNewSpecificationAsync(requirement);
            await Fixture.Requirements.SetMilestoneAsync(requirement, milestoneValue);

            // Add a test case
            var testSuite = await testSuiteManager.AddTestSuiteAsync(principal, new TestSuite { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId });
            var testCase = await testCaseManager.AddTestCaseAsync(principal, new TestCase { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId, TestSuiteId = testSuite.Id });

            // Link test and requirement
            await manager.AddRequirementLinkAsync(principal, requirement, testCase);
            TraceabilityNode traceabilityNode = await testCaseManager.DiscoverTraceabilityAsync(principal, testCase, 1);

            // Assert
            Assert.Single(traceabilityNode.Upstream);
            Assert.NotNull(traceabilityNode.Upstream[0].Requirement?.Id);
            Assert.Equal(requirement.Id, traceabilityNode.Upstream[0].Requirement!.Id);
        }
    }
}
