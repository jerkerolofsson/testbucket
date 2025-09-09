using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage.Settings
{
    class MinTargetSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public MinTargetSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "code-coverage-min-target";
            Metadata.Category.Name = "code";
            Metadata.Category.Icon = SettingIcons.Code;
            Metadata.Section.Name = "code-coverage";
            Metadata.Section.Icon = SettingIcons.Coverage;
            Metadata.SearchText = "code coverage target limit";
            Metadata.ShowDescription = false;
            Metadata.Grouping = "targets";
            Metadata.Type = FieldType.Integer;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            return new FieldValue { LongValue = settings.MinTarget, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            if (value.LongValue is not null && settings.MinTarget != value.LongValue)
            {
                settings.MinTarget = (int)value.LongValue.Value;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
            }
        }
    }
}
