using TestBucket.Contracts;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Tenants;
public interface ITenantRepository
{
    /// <summary>
    /// Creates a new tenant. Requires SUPERADMIN role
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name, string tenantId);
    Task<bool> ExistsAsync(string tenantId);
    Task<Tenant?> GetTenantByIdAsync(string tenantId);
    Task<PagedResult<Tenant>> SearchAsync(SearchQuery query);
}