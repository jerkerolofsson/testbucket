using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Settings.Appearance
{
    class ReducedMotionSetting : SettingAdapter
    { 
        private readonly IUserPreferencesManager _userPreferencesManager;

        public ReducedMotionSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "reduced-motion";
            Metadata.Description = "reduced-motion-description";
            Metadata.Category.Name = "appearance";
            Metadata.Category.Icon = SettingIcons.Appearance;
            Metadata.Section.Name = "accessibility";
            Metadata.Section.Icon = SettingIcons.Accessibility;
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
            return new FieldValue { BooleanValue = preferences.ReducedMotion, FieldDefinitionId = 0 };
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
            preferences.ReducedMotion = value.BooleanValue.Value;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
