using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Identity;
public interface IUserService
{
    Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, Predicate<ApplicationUser> predicate, CancellationToken cancellationToken = default);
}