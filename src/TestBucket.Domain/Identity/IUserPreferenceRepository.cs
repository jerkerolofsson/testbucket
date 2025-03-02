using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity;

public interface IUserPreferenceRepository
{
    Task<UserPreferences?> GetUserPreferencesAsync(string tenantId, string userName);
    Task SaveUserPreferencesAsync(UserPreferences userPreferences);
}