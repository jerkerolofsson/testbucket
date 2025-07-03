using System.Security.Claims;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
public interface IUserPreferencesManager
{
    event EventHandler<UserPreferences>? UserPreferencesChanged;

    Task<UserPreferences> LoadUserPreferencesAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Upserts settings
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="userPreferences"></param>
    /// <returns></returns>
    Task SaveUserPreferencesAsync(ClaimsPrincipal principal, UserPreferences userPreferences);
}