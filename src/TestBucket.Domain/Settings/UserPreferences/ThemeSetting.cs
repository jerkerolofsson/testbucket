using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Settings.Appearance
{
    class ThemeSetting : SettingAdapter
    {
        private readonly IUserPreferencesManager _userPreferencesManager;

        public ThemeSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "Theme";
            //Metadata.Description = "";
            Metadata.Category.Name = "Appearance";
            Metadata.Section.Name = "Theme";
            Metadata.Options = ["Default", "Yellow", "Hotpink"];
            Metadata.Type = FieldType.SingleSelection;
        }

        public override async Task<FieldValue> ReadAsync(ClaimsPrincipal principal)
        {
            if(principal.Identity?.Name is null)
            {
                return FieldValue.Empty;
            }

            var tenantId = principal.GetTentantIdOrThrow();
            var username = principal.Identity.Name;

            var preferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
            preferences ??= new Identity.Models.UserPreferences() { TenantId = tenantId, UserName = username };
            return new FieldValue { StringValue = preferences.Theme, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(ClaimsPrincipal principal, FieldValue value)
        {
            if (principal.Identity?.Name is null)
            {
                return;
            }

            var preferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
            preferences.Theme = value.StringValue;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
