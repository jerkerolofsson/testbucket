
namespace TestBucket.Domain.Tenants;

public interface ITenantManager
{
    Task UpdateTenantCiCdKeyAsync(string tenantId);
}