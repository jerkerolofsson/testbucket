using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Fields;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class RequirementsTestFramework(ProjectFixture Fixture)
    {
        internal async Task<RequirementSpecification> AddSpecificationAsync()
        {
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();

            var spec = new RequirementSpecification
            {
                Name = Guid.NewGuid().ToString(),
                TestProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId,
            };
            await requirementManager.AddRequirementSpecificationAsync(Impersonation.Impersonate(Fixture.App.Tenant), spec);
            return spec;
        }

        internal async Task AddRequirementToNewSpecificationAsync(Requirement requirement)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            requirement.RequirementSpecificationId = (await AddSpecificationAsync()).Id;
            requirement.TestProjectId = Fixture.ProjectId;
            requirement.TeamId = Fixture.TeamId;
            requirement.TenantId = Fixture.App.Tenant;

            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.AddRequirementAsync(user, requirement);
        }

        internal async Task SetMilestoneAsync(Requirement requirement, string milestoneName)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            // 
            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetRequirementFieldsAsync(principal, requirement.Id, fieldDefinitions);
            var milestoneField = fields.Where(x=>x.FieldDefinitionId == milestoneFieldDefinition.Id).First();
            milestoneField.StringValue = milestoneName;
            await fieldManager.UpsertRequirementFieldAsync(principal, milestoneField);
        }

        internal async Task<string?> GetMilestoneAsync(Requirement requirement)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var fieldDefinitionManager = Fixture.Services.GetRequiredService<IFieldDefinitionManager>();
            var fieldDefinitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneFieldDefinition = fieldDefinitions.Where(x => x.TraitType == TraitType.Milestone).First();

            // 
            var fieldManager = Fixture.Services.GetRequiredService<IFieldManager>();
            var fields = await fieldManager.GetRequirementFieldsAsync(principal, requirement.Id, fieldDefinitions);
            var milestoneField = fields.Where(x => x.FieldDefinitionId == milestoneFieldDefinition.Id).FirstOrDefault();
            return milestoneField?.StringValue;
        }
    }
}
