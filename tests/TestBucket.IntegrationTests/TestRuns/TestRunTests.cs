using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Core;

namespace TestBucket.IntegrationTests.TestRuns
{
    [FunctionalTest]
    [EnrichedTest]
    [IntegrationTest]
    public class TestRunTests(TestBucketApp App)
    {
        [Fact]
        [TestDescription("Verifies that a slug is generated when adding a new run")]
        public async Task AddRun_SlugIsCreated()
        {
            // Arrange
            var team = await App.Client.Teams.AddAsync("Team " + Guid.NewGuid().ToString());
            var project = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());
            try
            {
                var fieldDefinitions = await App.Client.Fields.GetProjectFieldDefinitionsAsync(project);

                // Add a test run with a milestone field (this is added by default)
                var inputRun = new TestRunDto { Team = team, Project = project, Name = "My run " + Guid.NewGuid().ToString() };

                var run = await App.Client.TestRuns.AddRunAsync(inputRun);
                Assert.NotNull(run);
                Assert.NotNull(run.Slug);


            }
            finally
            {
                // Cleanup
                await App.Client.Projects.DeleteAsync(project);
                await App.Client.Teams.DeleteAsync(team);
            }
        }

        [Fact]
        [TestDescription("Verifies that fields are inherited from a test run when adding a test run case")]
        public async Task AddTestRunCaseToTestRun_WithField_FieldIsAdded()
        {
            // Arrange
            var team = await App.Client.Teams.AddAsync("Team " + Guid.NewGuid().ToString());
            var project = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());
            try
            {
                var fieldDefinitions = await App.Client.Fields.GetProjectFieldDefinitionsAsync(project);

                // Add a test run with a milestone field (this is added by default)
                var milestoneValue = "1.0";
                var inputRun = new TestRunDto { Team = team, Project = project, Name = "My run " + Guid.NewGuid().ToString() };
                inputRun.Traits.Add(new TestTrait { Name = "Milestone", ExportType = TraitExportType.Instance, Type = TraitType.Milestone, Value = milestoneValue });

                var run = await App.Client.TestRuns.AddRunAsync(inputRun);
                Assert.NotNull(run);

                // Verify that the run has the milestone trait
                var milestoneTrait = run.Traits.Find(x => x.Type == TraitType.Milestone);
                Assert.NotNull(milestoneTrait);
                Assert.Equal(milestoneValue, milestoneTrait.Value);

            }
            finally
            {
                // Cleanup
                await App.Client.Projects.DeleteAsync(project);
                await App.Client.Teams.DeleteAsync(team);
            }
        }

        [Fact]
        [TestDescription("Verifies that milestone assigned with convenience trait property is handled properly and assigned as a field")]
        public async Task AddTestRunCaseToTestRun_WithMilestone_FieldIsAdded()
        {
            // Arrange
            var team = await App.Client.Teams.AddAsync("Team " + Guid.NewGuid().ToString());
            var project = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());
            try
            {
                var fieldDefinitions = await App.Client.Fields.GetProjectFieldDefinitionsAsync(project);

                // Add a test run with a milestone field (this is added by default)
                var milestoneValue = "1.0";
                var inputRun = new TestRunDto { Team = team, Project = project, Name = "My run " + Guid.NewGuid().ToString() };
                inputRun.Milestone = milestoneValue;

                var run = await App.Client.TestRuns.AddRunAsync(inputRun);
                Assert.NotNull(run);

                // Verify that the run has the milestone trait
                var milestoneTrait = run.Traits.Find(x => x.Type == TraitType.Milestone);
                Assert.NotNull(milestoneTrait);
                Assert.Equal(milestoneValue, milestoneTrait.Value);

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
