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
    class IncreasedContrastSetting : SettingAdapter
    { 
        private readonly IUserPreferencesManager _userPreferencesManager;

        public IncreasedContrastSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "Increased Contrast";
            Metadata.Description = "Increases color contrast to make the user interface easier to navigate";
            Metadata.Category.Name = "Accessibility";
            Metadata.Category.Icon = SettingIcon.Accessibility;
            Metadata.Section.Name = "Colors";
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
            return new FieldValue { BooleanValue = preferences.IncreasedContrast, FieldDefinitionId = 0 };
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
            preferences.IncreasedContrast = value.BooleanValue.Value;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
