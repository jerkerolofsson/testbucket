using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Settings.Appearance
{
    class PreferTextToIconsSetting : SettingAdapter
    { 
        private readonly IUserPreferencesManager _userPreferencesManager;

        public PreferTextToIconsSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "prefer-text-to-icons";
            Metadata.Description = "prefer-text-to-icons-description";
            Metadata.Category.Name = "accessibility";
            Metadata.Category.Icon = SettingIcon.Accessibility;
            Metadata.Section.Name = "appearance";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.Boolean;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var principal = context.Principal;
            if (principal.Identity?.Name is null)
            {
                return FieldValue.Empty;
            }

            var tenantId = principal.GetTenantIdOrThrow();
            var username = principal.Identity.Name;

            var preferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
            preferences ??= new Identity.Models.UserPreferences() { TenantId = tenantId, UserName = username };
            return new FieldValue { BooleanValue = preferences.PreferTextToIcons, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var principal = context.Principal;
            if (principal.Identity?.Name is null)
            {
                return;
            }

            if (value.BooleanValue is null)
            {
                return;
            }

            var preferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
            preferences.PreferTextToIcons = value.BooleanValue.Value;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
