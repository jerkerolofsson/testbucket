﻿using System.Security.Claims;

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
    /// Adds a personal API key where the permissions are an exact impersonation of the principal
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task AddPersonalApiKeyAsync(ClaimsPrincipal principal, ApplicationUserApiKey apiKey);

    /// <summary>
    /// Adds a project API key where the permissions are an exact impersonation of the principal and where a project claim is set
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task AddProjectApiKeyAsync(ClaimsPrincipal principal, long projectId, ApplicationUserApiKey apiKey);

    /// <summary>
    /// Adds an API key with a specific scope
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task AddApiKeyAsync(string scope, ClaimsPrincipal principal, ApplicationUserApiKey apiKey);
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
    Task<ApplicationUser?> GetUserByNormalizedUserNameAsync(ClaimsPrincipal principal, string normalizedUserName);

    /// <summary>
    /// Updates a user. principal must be the same user, or an admin
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task UpdateUserAsync(ClaimsPrincipal principal, ApplicationUser user);
    Task<ApplicationUser> GetSelfAsync(ClaimsPrincipal principal);


    Task AssignRoleAsync(ClaimsPrincipal principal, string normalizedEmail, string role);
    Task UnassignRoleAsync(ClaimsPrincipal principal, string normalizedEmail, string role);
    Task<IReadOnlyList<string>> GetRoleNamesAsync(ClaimsPrincipal principal);
    Task AddRoleAsync(ClaimsPrincipal principal, string name);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(ClaimsPrincipal principal, string normalizedUserName);
    Task DeleteUserAsync(ClaimsPrincipal principal, ApplicationUser user);
}