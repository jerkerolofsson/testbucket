

namespace TestBucket.Domain.Tenants;

public interface ITenantManager
{
    /// <summary>
    /// Creates q a new tenant 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(ClaimsPrincipal principal, string name);
    Task DeleteAsync(ClaimsPrincipal principal, string tenantId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns true if the tenant exists
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(ClaimsPrincipal principal, string tenantId);

    /// <summary>
    /// Returns a tenant by ID
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<Tenant?> GetTenantByIdAsync(ClaimsPrincipal principal, string tenantId);

    /// <summary>
    /// Searches for tenants
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<Tenant>> SearchAsync(ClaimsPrincipal principal, SearchQuery query);

    /// <summary>
    /// Generates a new CI/CD access token for the tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task UpdateTenantCiCdKeyAsync(string tenantId);
}