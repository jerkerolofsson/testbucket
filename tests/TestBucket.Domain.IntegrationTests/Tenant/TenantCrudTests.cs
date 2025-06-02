using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.IntegrationTests.Tenant
{
    [Component("Tenant")]
    [IntegrationTest]
    [FunctionalTest]
    public class TenantCrudTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        public async Task AddAndDeleteTenant_Successful()
        {
            var tenantManager = Fixture.Services.GetRequiredService<ITenantManager>();    
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            var result = await tenantManager.CreateAsync(user, "tenant-" + Guid.NewGuid().ToString());
            Assert.True(result.IsT0);

            await tenantManager.DeleteAsync(user, result.AsT0.Id, TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task DeleteTenant_WithProject_ProjectAlsoDeleted()
        {
            // Arrange
            var tenantManager = Fixture.Services.GetRequiredService<ITenantManager>();
            var projectManager = Fixture.Services.GetRequiredService<IProjectManager>();
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var result = await tenantManager.CreateAsync(user, "tenant-" + Guid.NewGuid().ToString());
            var tenantId = result.AsT0.Id;

            var tenantUser = Impersonation.Impersonate(tenantId);
            var project = new TestProject { Name = "name1" + Guid.NewGuid().ToString(), Slug = "", ShortName = "" };
            await projectManager.AddAsync(tenantUser, project);

            // Act 
            await tenantManager.DeleteAsync(user, result.AsT0.Id, TestContext.Current.CancellationToken);

            // Assert
            var projectAfterDelete = await projectManager.GetTestProjectByIdAsync(tenantUser, project.Id);
            Assert.Null(projectAfterDelete);
        }


        [Fact]
        public async Task AddTenant_CiCdTokenCreated()
        {
            var tenantManager = Fixture.Services.GetRequiredService<ITenantManager>();
            var user = Impersonation.Impersonate(Fixture.App.Tenant);

            var result = await tenantManager.CreateAsync(user, "tenant-" + Guid.NewGuid().ToString());
            Assert.True(result.IsT0);

            try
            {
                // Assert
                var tenant = await tenantManager.GetTenantByIdAsync(user, result.AsT0.Id);
                Assert.NotNull(tenant);
                Assert.NotNull(tenant.CiCdAccessToken);

            }
            finally
            {
                await tenantManager.DeleteAsync(user, result.AsT0.Id, TestContext.Current.CancellationToken);
            }
        }
    }
}
