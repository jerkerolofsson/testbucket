using Mediator;

using TestBucket.Contracts.TestResources;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Events;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.TestResources;
internal class TestResourceManager : ITestResourceManager
{
    private readonly TimeProvider _timeProvider;
    private readonly IMediator _mediator;
    private readonly ITestResourceRepository _repository;
    private const int MAX_RESOURCES_PER_OWNER = 256;

    public TestResourceManager(ITestResourceRepository repository, TimeProvider timeProvider, IMediator mediator)
    {
        _repository = repository;
        _timeProvider = timeProvider;
        _mediator = mediator;
    }


    /// <inheritdoc/>
    public async Task UpdateAsync(ClaimsPrincipal principal, TestResource resource)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestResource, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(resource.TenantId);
        resource.TenantId = principal.GetTenantIdOrThrow();
        resource.ModifiedBy = resource.CreatedBy;
        resource.Modified = _timeProvider.GetUtcNow();
        await _repository.UpdateAsync(resource);

        await _mediator.Publish(new TestResourceUpdated(principal, resource));
    }

    public async Task AddAsync(ClaimsPrincipal principal, TestResource resource)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestResource, PermissionLevel.Write);
        resource.TenantId = principal.GetTenantIdOrThrow();
        resource.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        resource.ModifiedBy = resource.CreatedBy;
        resource.Created =  resource.Modified = _timeProvider.GetUtcNow();
        await _repository.AddAsync(resource);

        await _mediator.Publish(new TestResourceAdded(principal, resource));
    }

    public async Task<PagedResult<TestResource>> BrowseAsync(ClaimsPrincipal principal, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestResource, PermissionLevel.Read);
        FilterSpecification<TestResource>[] filters = [new FilterByTenant<TestResource>(principal.GetTenantIdOrThrow())];
        return await _repository.SearchAsync(filters, offset, count);
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, TestResource resource)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestResource, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(resource.TenantId);
        await _repository.DeleteAsync(resource.Id);

        await _mediator.Publish(new TestResourceRemoved(principal, resource));
    }

    /// <summary>
    /// This is invoked by the resource server via a API and contains all resources for the server.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resources"></param>
    /// <returns></returns>
    public async Task UpdateResourcesFromResourceServerAsync(ClaimsPrincipal principal, List<TestResourceDto> resources)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        var owners = resources.Select(x => x.Owner).Distinct();

        foreach (var owner in owners.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!))
        {
            FilterSpecification<TestResource>[] filters = [
                new FilterByTenant<TestResource>(principal.GetTenantIdOrThrow()),
                new FindResourceByOwner(owner)
                ];
            var existingResources = await _repository.SearchAsync(filters, 0, MAX_RESOURCES_PER_OWNER);

            foreach(var resource in existingResources.Items)
            {
                var updatedResource = resources.Where(x => x.ResourceId == resource.ResourceId).FirstOrDefault();
                if(updatedResource is null)
                {
                    // Mark non existing resources as disabled
                    resource.Enabled = false;
                    await UpdateAsync(principal, resource);
                }
            }

            // Process all new or updated resources
            foreach(var resource in resources)
            {
                var existingResource = existingResources.Items.Where(x => x.ResourceId == resource.ResourceId).FirstOrDefault();

                var enabled = resource.Health == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
                if (existingResource is null)
                {
                    var testResource = new TestResource
                    {
                        Enabled = enabled,
                        Name = resource.Name,
                        Manufacturer = resource.Manufacturer,
                        Model = resource.Model,
                        Health = resource.Health,
                        ResourceId = resource.ResourceId,
                        Owner = resource.Owner,
                        TenantId = tenantId,
                        Types = resource.Types.ToArray(),
                        Variables = resource.Variables,
                    };
                    testResource.LastSeen = _timeProvider.GetUtcNow();
                    testResource.Variables["model"] = resource.Model ?? "";
                    testResource.Variables["manufacturer"] = resource.Manufacturer ?? "";
                    await AddAsync(principal, testResource);
                }
                else
                {
                    existingResource.Enabled = enabled;
                    existingResource.Model = resource.Model;
                    existingResource.Types = resource.Types.ToArray();
                    existingResource.Manufacturer = resource.Manufacturer;
                    existingResource.Variables = resource.Variables;
                    existingResource.LastSeen = _timeProvider.GetUtcNow();

                    existingResource.Variables["model"] = resource.Model ?? "";
                    existingResource.Variables["manufacturer"] = resource.Manufacturer ?? "";
                    await UpdateAsync(principal, existingResource);
                }
            }
        }
    }

    public async Task<TestResource?> GetByIdAsync(ClaimsPrincipal principal, long id)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetByIdAsync(tenantId, id);
    }
}
