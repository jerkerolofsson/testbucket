using TestBucket.Contracts;
using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
public interface IUserService
{
    Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, Predicate<ApplicationUser> predicate, CancellationToken cancellationToken = default);
}