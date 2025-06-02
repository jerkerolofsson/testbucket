using System.Runtime.CompilerServices;

namespace TestBucket.Domain.Tenants;
public interface ITenantRepository
{
    /// <summary>
    /// Creates a new tenant. 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name, string tenantId);
    Task<bool> ExistsAsync(string tenantId);
    Task<Tenant?> GetTenantByIdAsync(string tenantId);
    Task<PagedResult<Tenant>> SearchAsync(SearchQuery query);
    Task UpdateTenantAsync(Tenant tenant);

    /// <summary>
    /// Enumerates all items by fetching page-by-page
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<Tenant> EnumerateAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageSize = 20;
        var offset = 0;
        while(!cancellationToken.IsCancellationRequested)
        {
            var result = await SearchAsync(new SearchQuery() { Offset = offset, Count = pageSize });
            foreach(var item in result.Items)
            {
                yield return item;
            }
            if(result.Items.Length != pageSize)
            {
                break;
            }
            offset += result.Items.Length;
        }
    }

    Task DeleteTenantAsync(string tenantId);
}