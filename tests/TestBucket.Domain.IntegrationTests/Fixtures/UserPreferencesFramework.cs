using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class UserPreferencesFramework(ProjectFixture Fixture)
    {
        internal async Task<UserPreferences> LoadUserPreferencesAsync()
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IUserPreferencesManager>();

            return await manager.LoadUserPreferencesAsync(user);
        }

        internal async Task SaveUserPreferencesAsync(UserPreferences prefs)
        {
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IUserPreferencesManager>();

            await manager.SaveUserPreferencesAsync(user, prefs);
        }

    }
}
