using System.Security.Claims;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;
public interface IUserPreferencesManager
{
    event EventHandler<UserPreferences>? UserPreferencesChanged;

    Task<UserPreferences> LoadUserPreferencesAsync(ClaimsPrincipal principal);
    Task SaveUserPreferencesAsync(ClaimsPrincipal principal, UserPreferences userPreferences);
}