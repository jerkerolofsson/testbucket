using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Testing.Settings
{
    class ShowFailureMessageDialogWhenFailingTestCaseRunSetting : SettingAdapter
    {
        private readonly IUserPreferencesManager _userPreferencesManager;

        public ShowFailureMessageDialogWhenFailingTestCaseRunSetting(IUserPreferencesManager manager)
        {
            _userPreferencesManager = manager;

            Metadata.Name = "Show failure message dialog when failing a test";
            Metadata.Description = "Opens a dialog when failing a test, letting the tester enter a description of the failure";
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
            return new FieldValue { BooleanValue = preferences.ShowFailureMessageDialogWhenFailingTestCaseRun, FieldDefinitionId = 0 };
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
            preferences.ShowFailureMessageDialogWhenFailingTestCaseRun = value.BooleanValue.Value;

            await _userPreferencesManager.SaveUserPreferencesAsync(principal, preferences);
        }
    }
}
