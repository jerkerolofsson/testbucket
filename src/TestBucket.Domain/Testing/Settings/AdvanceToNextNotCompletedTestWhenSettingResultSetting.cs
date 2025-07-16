using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Testing.Settings
{
    class AdvanceToNextNotCompletedTestWhenSettingResultSetting : SettingAdapter
    {
        private readonly IUserPreferencesManager _userPreferencesManager;

        public AdvanceToNextNotCompletedTestWhenSettingResultSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "Automatically advance to next test when completed";
            Metadata.Description = "Opens the next incomplete test when a test is assigned a result";
            Metadata.Category.Name = "testing";
            Metadata.Category.Icon = SettingIcon.Testing;
            Metadata.Section.Name = "test-execution";
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
            return new FieldValue { BooleanValue = preferences.AdvanceToNextNotCompletedTestWhenSettingResult, FieldDefinitionId = 0 };
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
            preferences.AdvanceToNextNotCompletedTestWhenSettingResult = value.BooleanValue.Value;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
