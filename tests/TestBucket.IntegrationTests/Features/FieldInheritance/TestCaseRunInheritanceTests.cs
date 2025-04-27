using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Core;

namespace TestBucket.IntegrationTests.Features.FieldInheritance
{
    [Feature("Field Inheritance")]
    [FunctionalTest]
    [EnrichedTest]
    [IntegrationTest]
    public class TestCaseRunInheritanceTests(TestBucketApp App)
    {
        [Fact]
        [TestDescription("Verifies that fields are inherited from a test run when adding a test run case")]
        public async Task AddTestRunCaseToTestRun_WithInheritedFields_FieldsAreInherited()
        {
            // Arrange
            var team = await App.Client.Teams.AddAsync("Team " + Guid.NewGuid().ToString());
            var project = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());
            var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, "Suite " + Guid.NewGuid().ToString());
            var test = await App.Client.TestRepository.AddTestAsync(team, project, suite, "Test " + Guid.NewGuid().ToString());
            try
            {
                var milestoneValue = "1.0";
                var inputRun = new TestRunDto { Team = team, Project = project, Name = "My run " + Guid.NewGuid().ToString() };
                inputRun.Traits.Add(new TestTrait { Name = "Milestone", ExportType = TraitExportType.Instance, Type = TraitType.Milestone, Value = milestoneValue });

                var run = await App.Client.TestRuns.AddRunAsync(inputRun);
                Assert.NotNull(run);

                // Add a test case run to the test run: The milestone trait should be inherited from the run
                var testCaseRunToCreate = new TestCaseRunDto { TestCaseSlug = test, Name = Guid.NewGuid().ToString() };
                var testCaseRun = await App.Client.TestRuns.AddTestCaseRunAsync(run.Slug, testCaseRunToCreate);
            }
            finally
            {
                // Cleanup
                await App.Client.Projects.DeleteAsync(project);
                await App.Client.Teams.DeleteAsync(team);
            }
        }
    }
}
