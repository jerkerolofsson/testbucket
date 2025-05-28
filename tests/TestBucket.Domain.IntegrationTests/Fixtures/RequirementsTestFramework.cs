using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts;
using TestBucket.Data.Migrations;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.Models;
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

        internal async Task<Requirement> AddRequirementToNewSpecificationAsync()
        {
            var requirement = new Requirement { Name = Guid.NewGuid().ToString() };

            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            requirement.RequirementSpecificationId = (await AddSpecificationAsync()).Id;
            requirement.TestProjectId = Fixture.ProjectId;
            requirement.TeamId = Fixture.TeamId;
            requirement.TenantId = Fixture.App.Tenant;

            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.AddRequirementAsync(user, requirement);
            return requirement;
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

        internal async Task<RequirementSpecificationFolder> AddFolderAsync(string name, long specificationId)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var folder = new RequirementSpecificationFolder() { Name = name };
            folder.RequirementSpecificationId = specificationId;
            folder.TestProjectId = Fixture.ProjectId;
            folder.TeamId = Fixture.TeamId;
            folder.TenantId = Fixture.App.Tenant;

            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.AddFolderAsync(user, folder);
            return folder;
        }

        /// <summary>
        /// Returns requirements in a specification
        /// </summary>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        internal async Task<PagedResult<Requirement>> GetRequirementsAsync(long specificationId)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            return await requirementManager.SearchRequirementsAsync(user, new SearchRequirementQuery
            {
                Count = 1000,
                Offset = 0,
                RequirementSpecificationId = specificationId
            });
        }

        internal async Task UpdateAsync(RequirementSpecification spec)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.UpdateRequirementSpecificationAsync(user, spec);
        }

        internal async Task ExtractRequirementsFromSpecificationAsync(RequirementSpecification spec)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.ExtractRequirementsFromSpecificationAsync(user, spec, default);
        }

        internal async Task UpdateAsync(RequirementSpecificationFolder folder)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.UpdateFolderAsync(user, folder);
        }
        internal async Task UpdateAsync(Requirement requirement)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            await requirementManager.UpdateRequirementAsync(user, requirement);
        }

        internal async Task<Requirement?> GetRequirementByIdAsync(long id)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var requirementManager = Fixture.Services.GetRequiredService<IRequirementManager>();
            return await requirementManager.GetRequirementByIdAsync(user, id);
        }
    }
}
