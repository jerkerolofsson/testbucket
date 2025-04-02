using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;

/// <summary>
/// ADMIN API to access users
/// </summary>
public interface IUserManager
{
    #region API Keys
    /// <summary>
    /// Returns all API keys for a user
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task<ApplicationUserApiKey[]> GetApiKeysAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Deletes an API key
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task DeleteApiKeyAsync(ClaimsPrincipal principal, ApplicationUserApiKey apiKey);

    /// <summary>
    /// Adds an API key
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task AddApiKeyAsync(ClaimsPrincipal principal, ApplicationUserApiKey apiKey);
    #endregion

    /// <summary>
    /// Adds a user
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<IdentityResult> AddUserAsync(ClaimsPrincipal principal, string email, string password);

    /// <summary>
    /// Lists users
    /// Administrators can list users
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<ApplicationUser>> BrowseAsync(ClaimsPrincipal principal, int offset, int count);
    Task<ApplicationUser?> FindAsync(ClaimsPrincipal principal);
}