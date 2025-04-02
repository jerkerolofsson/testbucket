using System.Security.Claims;

using TestBucket.Contracts;
using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
public interface IUserService
{
    #region API Keys
    Task AddApiKeyAsync(ApplicationUserApiKey apiKey);
    Task DeleteApiKeyAsync(string userId, string tenantId, long apiKeyId);
    Task<ApplicationUserApiKey[]> GetApiKeysAsync(string userId, string tenantId);
    #endregion

    Task<ApplicationUser?> FindAsync(ClaimsPrincipal principal, string tenantId);
    Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> SearchAsync(string tenantId, SearchQuery query, Predicate<ApplicationUser> predicate, CancellationToken cancellationToken = default);
}