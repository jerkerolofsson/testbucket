using Mediator;

using TestBucket.Contracts;
using TestBucket.Contracts.TestResources;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>

    public class TestResourcesFramework(ProjectFixture Fixture)
    {
        internal async Task<TestResource> AddDisabledTestResourceAsync(string owner, string[] types)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();

            var resource = new TestResource { Name = Guid.NewGuid().ToString(), ResourceId = Guid.NewGuid().ToString(), Types = types, Owner = owner };
            resource.Enabled = false;
            resource.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
            await manager.AddAsync(Impersonation.Impersonate(Fixture.App.Tenant), resource);
            return resource;
        }

        internal async Task<TestResource> AddUnhealthyTestResourceAsync(string owner, string[] types)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();

            var resource = new TestResource { Name = Guid.NewGuid().ToString(), ResourceId = Guid.NewGuid().ToString(), Types = types, Owner = owner };
            resource.Enabled = true;
            resource.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy;
            await manager.AddAsync(Impersonation.Impersonate(Fixture.App.Tenant), resource);
            return resource;
        }

        internal async Task<TestResource> AddTestResourceAsync(string owner, string[] types)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();

            var resource = new TestResource { Name = Guid.NewGuid().ToString(), ResourceId = Guid.NewGuid().ToString(), Types = types, Owner = owner };
            resource.Enabled = true;
            resource.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
            await manager.AddAsync(Impersonation.Impersonate(Fixture.App.Tenant), resource);
            return resource;
        }

        internal async Task<TestResource> AddTestResourceAsync(string owner, string[] types, Dictionary<string,string> variables)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();

            var resource = new TestResource { Name = Guid.NewGuid().ToString(), Variables = variables, ResourceId = Guid.NewGuid().ToString(), Types = types, Owner = owner };
            resource.Enabled = true;
            resource.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
            await manager.AddAsync(Impersonation.Impersonate(Fixture.App.Tenant), resource);
            return resource;
        }


        internal async Task DeleteAsync(TestResource resource)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();

            await manager.DeleteAsync(Impersonation.Impersonate(Fixture.App.Tenant), resource);
        }

        internal async Task ReleaseAsync(string owner)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var mediator = Fixture.Services.GetRequiredService<IMediator>();
            await mediator.Send(new ReleaseResourcesRequest(owner, principal.GetTenantIdOrThrow()));
        }

        internal async Task<TestResourceBag> AllocateAsync(TestRun run, TestExecutionContext context)
        {
            var allocator = Fixture.Services.GetRequiredService<TestResourceDependencyAllocator>();
            var accountBag = await allocator.CollectDependenciesAsync(Impersonation.Impersonate(Fixture.App.Tenant), context);
            return accountBag;

        }
        internal async Task<TestResourceBag> AllocateAsync(TestRun run, string guid, IEnumerable<TestCaseDependency> dependencies)
        {
            var allocator = Fixture.Services.GetRequiredService<TestResourceDependencyAllocator>();

            var context = new TestExecutionContext
            {
                Guid = guid,
                ProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId,
                TestRunId = run.Id,
                Dependencies = new List<TestCaseDependency>(dependencies)
            };
            return await allocator.CollectDependenciesAsync(Impersonation.Impersonate(Fixture.App.Tenant), context, default);
        }

        internal async Task<TestResource?> GetByIdAsync(long id)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();

            return await manager.GetByIdAsync(Impersonation.Impersonate(Fixture.App.Tenant), id);
        }
        internal async Task<PagedResult<TestResource>> BrowseAsync()
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();
            return await manager.BrowseAsync(Impersonation.Impersonate(Fixture.App.Tenant), 0, 100);
        }

        internal async Task UpdateAsync(TestResource resource)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();
            await manager.UpdateAsync(Impersonation.Impersonate(Fixture.App.Tenant), resource);
        }

        internal async Task UpdateResourcesFromResourceServerAsync(TestResourceDto[] resources)
        {
            var manager = Fixture.Services.GetRequiredService<ITestResourceManager>();
            await manager.UpdateResourcesFromResourceServerAsync(Impersonation.Impersonate(Fixture.App.Tenant), resources.ToList());
        }
    }
}
