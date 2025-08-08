using Microsoft.AspNetCore.Http.HttpResults;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.IntegrationTests.Features.FieldInheritance
{
    /// <summary>
    /// Tests relaweted to field inheritance for requirements
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Requirements")]
    [Feature("Field Inheritance")]
    public class RequirementFieldInheritanceTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a requirement field is inherited from the parent requirement when a child requirement is created.
        /// 
        /// # Steps
        /// 1. Create a requirement specification
        /// 2. Create a parent requirement
        /// 3. Add a field to the parent requirement
        /// 4. Create a child requirement with the parent requirement
        /// 5. Verify that the child requirement has the same field value as the parent requirement
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddRequirement_WithParentRequirement_FieldsInherited()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var fieldManager = scope.ServiceProvider.GetRequiredService<IFieldManager>();
            var principal = Fixture.App.SiteAdministrator;
            var milestoneFieldDefinition = await Fixture.GetMilestoneFieldAsync();
            var milestoneValue = "1.0";

            // Arrange
            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec);

            // Parent requirement
            var requirement = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec.Id, TestProjectId = Fixture.ProjectId };
            await manager.AddRequirementAsync(principal, requirement);
            await fieldManager.UpsertRequirementFieldAsync(principal, new RequirementField { RequirementId = requirement.Id, FieldDefinitionId = milestoneFieldDefinition.Id, StringValue = milestoneValue });

            // Add child requirement
            var childRequirement = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec.Id, ParentRequirementId = requirement.Id, TestProjectId = Fixture.ProjectId };
            await manager.AddRequirementAsync(principal, childRequirement);

            // Assert
            var fields = await fieldManager.GetRequirementFieldsAsync(Fixture.App.SiteAdministrator, childRequirement.Id, []);
            Assert.NotEmpty(fields);
            var milestoneField = fields.Where(x => x.FieldDefinition?.TraitType == Traits.Core.TraitType.Milestone).FirstOrDefault();
            Assert.NotNull(milestoneField);
            Assert.Equal(milestoneValue, milestoneField.StringValue);
        }
    }
}
