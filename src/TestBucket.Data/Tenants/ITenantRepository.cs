
using OneOf;

using TestBucket.Data.Errors;
using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Tenants;
public interface ITenantRepository
{
    Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name, string tenantId);
    Task<bool> ExistsAsync(string tenantId);
    Task<PagedResult<Tenant>> SearchAsync(SearchQuery query);
}