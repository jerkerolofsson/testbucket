
using TestBucket.Components.Tenants;
using TestBucket.Domain.Identity;

namespace TestBucket.Components.Account;

internal class UserPreferencesService : TenantBaseService
{
    private readonly IUserPreferenceRepository _repo;
    private UserPreferences? _userPreferences = null;

    public UserPreferencesService(AuthenticationStateProvider authenticationStateProvider, IUserPreferenceRepository repo) : base(authenticationStateProvider)
    {
        _repo = repo;
    }

    public async Task<UserPreferences?> GetUserPreferencesAsync()
    {
        string tenantId = await GetTenantIdAsync();
        var userName = await GetUserNameAsync();
        if (userName is null)
        {
            // Not signed in, default preferences
            return new UserPreferences() { TenantId = tenantId, UserName = "", DarkMode = true };
        }
        if(_userPreferences is null || _userPreferences.TenantId != tenantId || _userPreferences.UserName != userName)
        {
            _userPreferences = await _repo.GetUserPreferencesAsync(tenantId, userName);
            _userPreferences ??= new() { TenantId = tenantId, UserName = userName };
        }
        return _userPreferences;
    }
    public async Task SaveUserPreferencesAsync(UserPreferences userPreferences)
    {
        var userName = await GetUserNameAsync();
        if (userName is not null)
        {
            userPreferences.TenantId = await GetTenantIdAsync();
            userPreferences.UserName = userName;
            _userPreferences = userPreferences;
            await _repo.SaveUserPreferencesAsync(userPreferences);
        }
    }
}
