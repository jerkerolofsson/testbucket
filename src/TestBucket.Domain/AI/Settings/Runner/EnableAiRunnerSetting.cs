using TestBucket.Domain.AI.Settings.Runner;

namespace TestBucket.Domain.AI.Settings
{
    class EnableAiRunnerSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public EnableAiRunnerSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "ai-runner-enabled";
            Metadata.Description = "ai-runner-enabled-description";
            Metadata.Category.Name = "testing";
            Metadata.Category.Icon = SettingIcon.Testing;
            Metadata.Section.Name = "ai-runner";
            Metadata.SearchText = "ai-runner";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.Boolean;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<AiRunnerSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            return new FieldValue { BooleanValue = settings.Enabled, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<AiRunnerSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            if (settings.Enabled != value.BooleanValue)
            {
                settings.Enabled = value.BooleanValue ?? false;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
            }
        }
    }
}
