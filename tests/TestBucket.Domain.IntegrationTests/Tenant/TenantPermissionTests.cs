using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.IntegrationTests.Tenant
{
    [Component("Tenant")]
    [IntegrationTest]
    [SecurityTest]
    public class TenantPermissionTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that an UnauthorizedAccessException is thrown if trying to add a tenant without write permission
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddTenant_WithoutWritePermission_NotSuccessful()
        {
            // Arrange
            var tenantManager = Fixture.Services.GetRequiredService<ITenantManager>();
            var projectManager = Fixture.Services.GetRequiredService<IProjectManager>();
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            var tenantUser = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.UserName = "no-write-permission";
                builder.Add(PermissionEntityType.Tenant, PermissionLevel.Read);
            });
            
            // Act/Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                var result = await tenantManager.CreateAsync(tenantUser, "tenant-" + Guid.NewGuid().ToString());
                var tenantId = result.AsT0.Id;
            });
        }

        [Fact]
        public async Task DeleteTenant_WithoutDeletePermission_NotSuccessful()
        {
            // Arrange
            var tenantManager = Fixture.Services.GetRequiredService<ITenantManager>();
            var projectManager = Fixture.Services.GetRequiredService<IProjectManager>();
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var result = await tenantManager.CreateAsync(user, "tenant-" + Guid.NewGuid().ToString());
            var tenantId = result.AsT0.Id;

            // Act/Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                var tenantUser = Impersonation.Impersonate(builder =>
                {
                    builder.TenantId = tenantId;
                    builder.UserName = "no-write-permission";
                    builder.Add(PermissionEntityType.Tenant, PermissionLevel.ReadWriteApprove);
                });

                await tenantManager.DeleteAsync(tenantUser, tenantId, TestContext.Current.CancellationToken);
            });
        }
    }
}
