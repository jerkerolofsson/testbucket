using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>
    public class UserPreferencesFramework(ProjectFixture Fixture)
    {
        internal async Task<UserPreferences> LoadUserPreferencesAsync()
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            return await LoadUserPreferencesAsync(user);
        }

        internal async Task<UserPreferences> LoadUserPreferencesAsync(System.Security.Claims.ClaimsPrincipal user)
        {
            var manager = Fixture.Services.GetRequiredService<IUserPreferencesManager>();
            return await manager.LoadUserPreferencesAsync(user);
        }

        internal async Task SaveUserPreferencesAsync(UserPreferences prefs)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            await SaveUserPreferencesAsync(prefs, user);
        }

        internal async Task SaveUserPreferencesAsync(UserPreferences prefs, System.Security.Claims.ClaimsPrincipal user)
        {
            var manager = Fixture.Services.GetRequiredService<IUserPreferencesManager>();
            await manager.SaveUserPreferencesAsync(user, prefs);
        }
    }
}
