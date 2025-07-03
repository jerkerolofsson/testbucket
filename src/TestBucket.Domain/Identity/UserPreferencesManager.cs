using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity
{
    internal class UserPreferencesManager : IUserPreferencesManager
    {
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private UserPreferences? _preferences;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public event EventHandler<UserPreferences>? UserPreferencesChanged;


        public UserPreferencesManager(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        public async Task<UserPreferences> LoadUserPreferencesAsync(ClaimsPrincipal principal)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var username = principal.Identity?.Name ?? throw new ArgumentException("User not authenticated");

            await _lock.WaitAsync();
            try
            {
                var cachedPreferences = _preferences;
                if (cachedPreferences is not null && cachedPreferences.UserName == username)
                {
                    return cachedPreferences;
                }
                var preferences = await _userPreferenceRepository.GetUserPreferencesAsync(tenantId, username);

                // Assign default values
                if (preferences is null)
                {
                    preferences ??= new UserPreferences { TenantId = tenantId, UserName = username };
                    preferences.KeyboardBindings ??= new();
                    if (preferences.KeyboardBindings.UnifiedSearchBinding is null)
                    {
                        preferences.KeyboardBindings.UnifiedSearchBinding = new Keyboard.KeyboardBinding() { CommandId = "none", Key = "Slash", ModifierKeys = Keyboard.ModifierKey.None };
                    }
                    await _userPreferenceRepository.SaveUserPreferencesAsync(preferences);
                }

                _preferences = preferences;

                return preferences;
            }
            finally
            {
                _lock.Release();
            }
        }
        public async Task SaveUserPreferencesAsync(ClaimsPrincipal principal, UserPreferences userPreferences)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var username = principal.Identity?.Name ?? throw new ArgumentException("User not authenticated");

            if (userPreferences.TenantId != tenantId)
            {
                throw new ArgumentException("Cannot save data from another user");
            }
            if (userPreferences.UserName != username)
            {
                throw new ArgumentException($"Cannot save data from another user: username:{username}, preferences: {userPreferences.UserName}");
            }

            await _lock.WaitAsync();
            try
            {
                _preferences = userPreferences;
                await _userPreferenceRepository.SaveUserPreferencesAsync(userPreferences);
            }
            finally
            {
                _lock.Release();
            }

            UserPreferencesChanged?.Invoke(this, userPreferences);
        }
    }
}
