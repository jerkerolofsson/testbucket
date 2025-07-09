using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Settings.Appearance
{
    class IncreasedFontSizeSetting : SettingAdapter
    { 
        private readonly IUserPreferencesManager _userPreferencesManager;

        public IncreasedFontSizeSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "increased-font-size";
            Metadata.Description = "incrased-font-size-description";
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
            return new FieldValue { BooleanValue = preferences.IncreasedFontSize, FieldDefinitionId = 0 };
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
            preferences.IncreasedFontSize = value.BooleanValue.Value;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
