using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Features.Traceability.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.IntegrationTests.Features.Traceability
{
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Traceability")]
    public class TestCaseTraceabilityTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [CoveredRequirement("requirement-traceability")]
        [TestDescription("""
            Verifies that when collecting a trace for a *testcase* with a "test case linked to a requirement" the **requirement**
            is returned.

            # Steps
            1. Create a requirement specification
            2. Create a requirement
            3. Add a milestone field to the requirement
            4. Create a test suite
            5. Create a test case
            6. Link the test case to the requirement
            7. Discover traceability for the test-case
            8. Assert that the requirement is included in the traceability node
            """)]
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
