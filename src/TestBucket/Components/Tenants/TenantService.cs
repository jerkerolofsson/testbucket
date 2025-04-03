using Microsoft.AspNetCore.Components.Authorization;

using OneOf;

using TestBucket.Components.Shared;
using TestBucket.Contracts;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Components.Tenants;

public class TenantController : SuperAdminGuard
{
    private readonly ITenantRepository _repository;

    public TenantController(
        ITenantRepository repository,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _repository = repository;
    }

    public async Task<Tenant?> GetTenantAsync()
    {
        var tenantId = await GetTenantIdAsync();
        if(tenantId is null)
        {
            return null;
        }
        return await _repository.GetTenantByIdAsync(tenantId);
    }

    public async Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            return new AlreadyExistsError();
        }
        var tenantId = new Slugify.SlugHelper().GenerateSlug(name);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return new AlreadyExistsError();
        }

        return await _repository.CreateAsync(name, tenantId);
    }

    public async Task<PagedResult<Tenant>> SearchAsync(SearchQuery query)
    {
        await GuardAsync();
        return await _repository.SearchAsync(query);
    }
}
