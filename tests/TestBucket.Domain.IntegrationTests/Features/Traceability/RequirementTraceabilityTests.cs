using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Data.Migrations;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.IntegrationTests.Fixtures;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.Traceability.Models;
using TestBucket.Traits.Xunit;
using Xunit;
using static Xunit.Sdk.TestOptionsNames;

namespace TestBucket.Domain.IntegrationTests.Features.Traceability
{
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Traceability")]
    public class RequirementTraceabilityTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        ///    Verifies that when collecting a trace for a *requirement* with a "test case linked to a requirement" the **test case**
        ///        is returned.
        ///    # Steps
        ///    1. Create a requirement specification
        ///    2. Create a requirement
        ///    3. Add a milestone field to the requirement
        ///    4. Create a test suite
        ///    5. Create a test case
        ///    6. Link the test case to the requirement
        ///    7. Discover traceability for the requirement
        ///    8. Assert that the test case is included in the traceability node
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("requirement-traceability")]
        public async Task GetRequirementTrace_WithTestLink_LinkIncluded()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var testSuiteManager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();
            var testCaseManager = scope.ServiceProvider.GetRequiredService<ITestCaseManager>();
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
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
            TraceabilityNode traceabilityNode = await manager.DiscoverTraceabilityAsync(principal, requirement, 1);

            // Assert
            Assert.Single(traceabilityNode.Downstream);
            Assert.NotNull(traceabilityNode.Downstream[0].TestCase?.Id);
            Assert.Equal(testCase.Id, traceabilityNode.Downstream[0].TestCase!.Id);
        }
    }
}
