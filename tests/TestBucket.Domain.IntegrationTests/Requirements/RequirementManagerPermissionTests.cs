

namespace TestBucket.Domain.IntegrationTests.Requirements
{
    [IntegrationTest]
    [EnrichedTest]
    public class RequirementManagerPermissionTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [SecurityTest]
        public async Task AddRequirementSpecification_WithPermission_IsSuccess()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var principal = Fixture.App.SiteAdministrator;

            var requirementSpec = new RequirementSpecification { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };
            await manager.AddRequirementSpecificationAsync(principal, requirementSpec);
        }

        [Fact]
        [SecurityTest]
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
        [SecurityTest]
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
        [SecurityTest]
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
