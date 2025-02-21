using Microsoft.AspNetCore.Components.Authorization;

using OneOf;

using TestBucket.Components.Shared;
using TestBucket.Contracts;
using TestBucket.Data.Errors;
using TestBucket.Data.Tenants;

namespace TestBucket.Components.Tenants;

public class TenantService : SuperAdminGuard
{
    private readonly ITenantRepository _repository;

    public TenantService(
        ITenantRepository repository,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _repository = repository;
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
