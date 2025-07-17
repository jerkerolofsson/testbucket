using TestBucket.Domain.AI.Settings.Runner;

namespace TestBucket.Domain.AI.Settings
{
    class MaxTokensPerDayForAitRunnerSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public MaxTokensPerDayForAitRunnerSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "ai-runner-max-tokens-per-day";
            Metadata.Description = "ai-runner-max-tokens-per-day-description";
            Metadata.Category.Name = "testing";
            Metadata.Category.Icon = SettingIcons.Testing;
            Metadata.Section.Name = "ai-runner";
            Metadata.Section.Icon = SettingIcons.AI;
            Metadata.SearchText = "ai-runner";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.Integer;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<AiRunnerSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            return new FieldValue { LongValue = settings.MaxTokensPerDay, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<AiRunnerSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            if (value.LongValue is not null && settings.MaxTokensPerDay != value.LongValue)
            {
                settings.MaxTokensPerDay = value.LongValue.Value;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
            }
        }
    }
}
