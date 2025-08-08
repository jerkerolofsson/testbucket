

namespace TestBucket.Domain.IntegrationTests.Requirements
{
    /// <summary>
    /// Tests related to permissions when managing requirements
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [Component("Requirements")]
    public class RequirementManagerPermissionTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that a requirement/collection can be added when the user has permissions
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Verifies that requirement specification/collection can be deleted if the user has permissi9ons
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Verifies that an UnauthorizedAccessException is thrown if the user tries to add a requirement specification when they only have read permissions.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Verifies that a UnauthorizedAccessException is thrown if the user tries to delete a requirement specification when they only have read permissions.
        /// </summary>
        /// <returns></returns>
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
