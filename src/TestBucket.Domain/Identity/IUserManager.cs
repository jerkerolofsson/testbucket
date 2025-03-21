using System.Security.Claims;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;

/// <summary>
/// ADMIN API to access users
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Lists users
    /// Administrators can list users
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<ApplicationUser>> BrowseAsync(ClaimsPrincipal principal, int offset, int count);
}