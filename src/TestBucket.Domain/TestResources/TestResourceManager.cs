using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.TestResources;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.TestResources;
internal class TestResourceManager : ITestResourceManager
{
    private readonly ITestResourceRepository _repository;
    private const int MAX_RESOURCES_PER_OWNER = 256;

    public TestResourceManager(ITestResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task UpdateAsync(ClaimsPrincipal principal, TestResource resource)
    {
        principal.ThrowIfEntityTenantIsDifferent(resource.TenantId);
        resource.TenantId = principal.GetTenantIdOrThrow();
        resource.ModifiedBy = resource.CreatedBy;
        resource.Modified = DateTimeOffset.UtcNow;
        await _repository.UpdateAsync(resource);
    }

    public async Task AddAsync(ClaimsPrincipal principal, TestResource resource)
    {
        resource.TenantId = principal.GetTenantIdOrThrow();
        resource.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        resource.ModifiedBy = resource.CreatedBy;
        resource.Created = DateTimeOffset.UtcNow;
        resource.Modified = DateTimeOffset.UtcNow;
        await _repository.AddAsync(resource);
    }

    public async Task<PagedResult<TestResource>> BrowseAsync(ClaimsPrincipal principal, int offset, int count)
    {
        FilterSpecification<TestResource>[] filters = [new FilterByTenant<TestResource>(principal.GetTenantIdOrThrow())];
        return await _repository.SearchAsync(filters, offset, count);
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, TestResource resource)
    {
        principal.ThrowIfEntityTenantIsDifferent(resource.TenantId);
        await _repository.DeleteAsync(resource.Id);
    }

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
            foreach(var resource in resources.Where(x => !existingResources.Items.Any(e => e.ResourceId == x.ResourceId)))
            {
                var existingResource = existingResources.Items.Where(x => x.ResourceId == resource.ResourceId).FirstOrDefault();

                if (existingResource is null)
                {
                    var enabled = resource.Health == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
                    var testResource = new TestResource
                    {
                        Enabled = true,
                        Name = resource.Name,
                        Manufacturer = resource.Manufacturer,
                        Model = resource.Model,
                        ResourceId = resource.ResourceId,
                        Owner = resource.Owner,
                        TenantId = tenantId,
                        Types = resource.Types,
                        Variables = resource.Variables,
                    };
                    await AddAsync(principal, testResource);
                }
                else
                {
                    existingResource.Enabled = false;

                    existingResource.Model = resource.Model;
                    existingResource.Types = resource.Types;
                    existingResource.Manufacturer = resource.Manufacturer;
                    existingResource.Variables = resource.Variables;
                    await UpdateAsync(principal, existingResource);
                }
            }
        }
    }
}
