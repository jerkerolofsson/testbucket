using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.IntegrationTests.Fixtures;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.Domain.IntegrationTests.Requirements
{
    [IntegrationTest]
    [EnrichedTest]
    public class RequirementManagerPermissionTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task AddRequirementSpecification_WithPermission_IsSuccess()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec);
        }

        [Fact]
        [FunctionalTest]
        public async Task DeleteRequirementSpecification_WithPermission_IsSuccess()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec);
            await manager.DeleteRequirementSpecificationAsync(principal, requirementSpec);
        }

        [Fact]
        [FunctionalTest]
        public async Task AddRequirementSpecification_WithOnlyReadPermission_Unauthorized()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
            });

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.AddRequirementSpecificationAsync(principal, requirementSpec);
            });
        }

        [Fact]
        [FunctionalTest]
        public async Task DeleteRequirementSpecification_WithOnlyReadPermission_Unauthorized()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
            });

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(Fixture.App.SiteAdministrator, requirementSpec);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.DeleteRequirementSpecificationAsync(principal, requirementSpec);
            });
        }
    }
}
