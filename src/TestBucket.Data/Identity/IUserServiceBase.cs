using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Identity;
public interface IUserServiceBase
{
    Task<PagedResult<ApplicationUser>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> SearchAsync(SearchQuery query, Predicate<ApplicationUser> predicate, CancellationToken cancellationToken = default);

}