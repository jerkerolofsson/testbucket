using Microsoft.AspNetCore.Components.Authorization;

using OneOf;

using TestBucket.Components.Shared;
using TestBucket.Contracts;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Components.Tenants;

internal class TenantController : SuperAdminGuard
{
    private readonly ITenantRepository _repository;
    private readonly ITenantManager _tenantManager;

    public TenantController(
        ITenantRepository repository,
        ITenantManager tenantManager,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _repository = repository;
        _tenantManager = tenantManager;
    }

    /// <summary>
    /// Returns the current tenant
    /// </summary>
    /// <returns></returns>
    public async Task<Tenant?> GetTenantAsync()
    {
        var tenantId = await GetTenantIdAsync();
        if(tenantId is null)
        {
            return null;
        }
        return await _repository.GetTenantByIdAsync(tenantId);
    }

    public Task<string?> GetDefaultTenantAsync()
    {
        return Task.FromResult(Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT));
    }
    public async Task<bool> ExistsAsync(string tenantId)
    {
        return await _repository.ExistsAsync(tenantId);
    }

    public async Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name)
    {
        // Check permissions
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Write);

        if(string.IsNullOrWhiteSpace(name))
        {
            return new AlreadyExistsError();
        }
        var tenantId = new Slugify.SlugHelper().GenerateSlug(name);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return new AlreadyExistsError();
        }

        await _repository.CreateAsync(name, tenantId);

        // Generate key for the tenant
        await _tenantManager.UpdateTenantCiCdKeyAsync(tenantId);

        return await _repository.GetTenantByIdAsync(tenantId) ?? throw new Exception("Failed to create tenant");
    }

    public async Task<PagedResult<Tenant>> SearchAsync(SearchQuery query)
    {
        // Check permissions
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        return await _repository.SearchAsync(query);
    }
}
