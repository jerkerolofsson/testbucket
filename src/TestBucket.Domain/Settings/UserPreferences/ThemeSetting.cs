using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Settings.Appearance
{
    class ThemeSetting : SettingAdapter
    {
        private readonly IUserPreferencesManager _userPreferencesManager;

        public ThemeSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "theme";
            //Metadata.Description = "";
            Metadata.Category.Name = "appearance";
            Metadata.Category.Icon = SettingIcons.Appearance;
            Metadata.Section.Name = "theme";
            Metadata.Section.Icon = SettingIcons.Theme;
            //Metadata.Options = ["Default", "Blue Steel", "Retro", "Winter", "Dark Moon", "Material", "Le Trigre"];
            Metadata.Type = FieldType.SingleSelection;
            Metadata.DataSourceType = Contracts.Fields.FieldDataSourceType.Theme;
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
            return new FieldValue { StringValue = preferences.Theme, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var principal = context.Principal;
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
