using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage.Settings
{
    class AutomaticallyImportTestRunCodeCoverageReportsSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AutomaticallyImportTestRunCodeCoverageReportsSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "code-coverage-auto-import";
            Metadata.Description = "code-coverage-auto-import-description";
            Metadata.ShowDescription = true;
            Metadata.Category.Name = "code";
            Metadata.Category.Icon = SettingIcons.Code;
            Metadata.Section.Name = "code-coverage";
            Metadata.Section.Icon = SettingIcons.Coverage;
            Metadata.SearchText = "code coverage import auto";
            Metadata.Grouping = "targets";
            Metadata.Type = FieldType.Boolean;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
            var settings = await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            return new FieldValue { BooleanValue = settings.AutomaticallyImportTestRunCodeCoverageReports, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<CodeCoverageSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
            settings ??= new();

            if (value.BooleanValue is not null && settings.AutomaticallyImportTestRunCodeCoverageReports != value.BooleanValue)
            {
                settings.AutomaticallyImportTestRunCodeCoverageReports = value.BooleanValue.Value;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
            }
        }
    }
}
