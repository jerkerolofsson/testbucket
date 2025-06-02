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
        var principal = await GetUserClaimsPrincipalAsync();
        return await _tenantManager.GetTenantByIdAsync(principal, tenantId);
    }

    public Task<string?> GetDefaultTenantAsync()
    {
        return Task.FromResult(Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT));
    }
    public async Task<bool> ExistsAsync(string tenantId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _tenantManager.ExistsAsync(principal, tenantId);
    }

    /// <summary>
    /// Creates a new tenant
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _tenantManager.CreateAsync(principal, name);
    }

    public async Task<PagedResult<Tenant>> SearchAsync(SearchQuery query)
    {
        // Check permissions
        var principal = await GetUserClaimsPrincipalAsync();
        return await _tenantManager.SearchAsync(principal, query);
    }
}
