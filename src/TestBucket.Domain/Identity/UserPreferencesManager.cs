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

        public event EventHandler<UserPreferences>? UserPreferencesChanged;

        public UserPreferencesManager(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        public async Task<UserPreferences> LoadUserPreferencesAsync(ClaimsPrincipal principal)
        {
            var tenantId = principal.GetTentantIdOrThrow();
            var username = principal.Identity?.Name ?? throw new ArgumentException("User not authenticated");
            var preferences = await _userPreferenceRepository.GetUserPreferencesAsync(tenantId, username);

            preferences ??= new UserPreferences { TenantId = tenantId, UserName = username };

            return preferences;
        }
        public async Task SaveUserPreferencesAsync(ClaimsPrincipal principal, UserPreferences userPreferences)
        {
            var tenantId = principal.GetTentantIdOrThrow();
            var username = principal.Identity?.Name ?? throw new ArgumentException("User not authenticated");

            if (userPreferences.TenantId != tenantId)
            {
                throw new ArgumentException("Cannot save data from another user");
            }
            if (userPreferences.UserName != username)
            {
                throw new ArgumentException("Cannot save data from another user");
            }

            await _userPreferenceRepository.SaveUserPreferencesAsync(userPreferences);

            UserPreferencesChanged?.Invoke(this, userPreferences);
        }
    }
}
