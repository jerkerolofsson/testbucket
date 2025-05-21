using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.Identity
{
    internal class UserPreferencesManager : IUserPreferencesManager
    {
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private UserPreferences? _preferences;

        public event EventHandler<UserPreferences>? UserPreferencesChanged;

        public UserPreferencesManager(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        public async Task<UserPreferences> LoadUserPreferencesAsync(ClaimsPrincipal principal)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var username = principal.Identity?.Name ?? throw new ArgumentException("User not authenticated");

            var cachedPreferences = _preferences;
            if (cachedPreferences is not null && cachedPreferences.UserName == username)
            {
                return cachedPreferences;
            }

            var preferences = await _userPreferenceRepository.GetUserPreferencesAsync(tenantId, username);

            // Assign default values
            preferences ??= new UserPreferences { TenantId = tenantId, UserName = username };
            preferences.KeyboardBindings ??= new();
            if(preferences.KeyboardBindings.UnifiedSearchBinding is null)
            {
                preferences.KeyboardBindings.UnifiedSearchBinding = new Keyboard.KeyboardBinding() { CommandId = "none", Key = "Slash", ModifierKeys = Keyboard.ModifierKey.None };
            }

            _preferences = preferences;

            return preferences;
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

            _preferences = userPreferences;
            await _userPreferenceRepository.SaveUserPreferencesAsync(userPreferences);

            UserPreferencesChanged?.Invoke(this, userPreferences);
        }
    }
}
