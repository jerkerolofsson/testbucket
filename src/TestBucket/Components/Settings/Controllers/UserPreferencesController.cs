using TestBucket.Domain.Identity;

namespace TestBucket.Components.Settings.Controllers;

internal class UserPreferencesController : TenantBaseService
{
    private readonly IUserPreferencesManager _userPreferencesManager;

    public UserPreferencesController(AuthenticationStateProvider authenticationStateProvider, IUserPreferencesManager userPreferencesManager)
        : base(authenticationStateProvider)
    {
        _userPreferencesManager = userPreferencesManager;
    }

    public async Task<UserPreferences> LoadUserPreferencesAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var userPreferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
        return userPreferences;
    }
    public async Task SaveUserPreferencesAsync(UserPreferences userPreferences)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _userPreferencesManager.SaveUserPreferencesAsync(principal, userPreferences);
    }
}
