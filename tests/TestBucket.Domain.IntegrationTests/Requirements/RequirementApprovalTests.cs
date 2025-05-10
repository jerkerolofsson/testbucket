using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class RequirementApprovalTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task ApproveRequirement_WithPermission_Success()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec);

            var requirement = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec.Id };
            await manager.AddRequirementAsync(principal, requirement);

            // Act
            await manager.ApproveRequirementAsync(principal, requirement);
        }

        [Fact]
        [SecurityTest]
        [TestDescription("""
            Verifies that a user without approval permission for requirements cannot approve a requirement
            """)]
        public async Task ApproveRequirement_WithoutPermission_Failure()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
                builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.Write);
                builder.Add(PermissionEntityType.Project, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.Write);
            });

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(user, requirementSpec);

            var requirement = new Requirement { Name = Guid.NewGuid().ToString(), RequirementSpecificationId = requirementSpec.Id };
            await manager.AddRequirementAsync(user, requirement);

            // Act
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.ApproveRequirementAsync(user, requirement);
            });
            
        }
    }
}
