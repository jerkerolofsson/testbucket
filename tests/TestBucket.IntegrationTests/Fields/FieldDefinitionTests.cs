using TestBucket.Contracts.Fields;

namespace TestBucket.IntegrationTests.Fields
{
    [Feature("Fields")]
    [FunctionalTest]
    [EnrichedTest]
    [IntegrationTest]
    public class FieldDefinitionTests(TestBucketApp App)
    {
        [Fact]
        [TestDescription("Verifies that a new project has default fields")]
        public async Task CreateProject_DefaultFieldsAreAdded()
        {
            var team = await App.Client.Teams.AddAsync("team " + Guid.NewGuid().ToString());
            try
            {
                var slug = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());

                var fields = await App.Client.Fields.GetProjectFieldDefinitionsAsync(slug);

                // Verify that it is not empty
                Assert.NotEmpty(fields);

                // Note: Static fields defined in SystemFieldDefinitions
                var feature = fields.Where(x => x.TraitType == Traits.Core.TraitType.Feature).FirstOrDefault();
                var milestone = fields.Where(x => x.TraitType == Traits.Core.TraitType.Milestone).FirstOrDefault();
                var commit = fields.Where(x => x.TraitType == Traits.Core.TraitType.Commit).FirstOrDefault();

                Assert.NotNull(feature);
                Assert.NotNull(milestone);
                Assert.NotNull(commit);

                await App.Client.Projects.DeleteAsync(slug);

            }
            finally
            {
                await App.Client.Teams.DeleteAsync(team);
            }
        }

        [Fact]
        [TestDescription("Verifies that a new project has default fields")]
        public async Task MilestoneField_AsDefaultProjectField_IsTargetedCorrectly()
        {
            var team = await App.Client.Teams.AddAsync("team " + Guid.NewGuid().ToString());
            try
            {
                var slug = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());

                var fields = await App.Client.Fields.GetProjectFieldDefinitionsAsync(slug);

                // Verify that it is not empty
                Assert.NotEmpty(fields);

                // Note: Static fields defined in SystemFieldDefinitions
                var milestone = fields.Where(x => x.TraitType == Traits.Core.TraitType.Milestone).FirstOrDefault();
                Assert.NotNull(milestone);

                Assert.True((milestone.Target & FieldTarget.TestRun) == FieldTarget.TestRun);
                Assert.True((milestone.Target & FieldTarget.TestCaseRun) == FieldTarget.TestCaseRun);
                Assert.True((milestone.Target & FieldTarget.Requirement) == FieldTarget.Requirement);
                Assert.True((milestone.Target & FieldTarget.RequirementSpecificationFolder) == FieldTarget.RequirementSpecificationFolder);
                Assert.True((milestone.Target & FieldTarget.RequirementSpecification) == FieldTarget.RequirementSpecification);

                await App.Client.Projects.DeleteAsync(slug);

            }
            finally
            {
                await App.Client.Teams.DeleteAsync(team);
            }
        }
    }
}
