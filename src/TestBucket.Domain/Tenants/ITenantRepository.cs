using TestBucket.Contracts;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Tenants;
public interface ITenantRepository
{
    Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name, string tenantId);
    Task<bool> ExistsAsync(string tenantId);
    Task<PagedResult<Tenant>> SearchAsync(SearchQuery query);
}