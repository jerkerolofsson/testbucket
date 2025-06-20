
using TestBucket.Contracts.Appearance.Models;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Settings.Appearance
{
    class ExplorerDockSetting : SettingAdapter
    {
        private readonly IUserPreferencesManager _userPreferencesManager;

        public ExplorerDockSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "explorer-dock";
            Metadata.Category.Name = "appearance";
            Metadata.Category.Icon = SettingIcon.Appearance;
            Metadata.Section.Name = "Layout";
            Metadata.Type = FieldType.Integer;
            Metadata.DataSourceType = Contracts.Fields.FieldDataSourceType.Dock;
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

            preferences.ExplorerDock ??= Dock.Left;

            return new FieldValue { LongValue = (long)preferences.ExplorerDock, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var principal = context.Principal;
            if (principal.Identity?.Name is null)
            {
                return;
            }

            if (value.LongValue is null)
            {
                return;
            }

            var preferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
            preferences.ExplorerDock = (Dock)(value.LongValue ?? 1);

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
