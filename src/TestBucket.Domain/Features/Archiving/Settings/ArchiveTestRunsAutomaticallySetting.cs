using TestBucket.Domain.Features.Archiving.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Testing.Settings
{
    class ArchiveTestRunsAutomaticallySetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public ArchiveTestRunsAutomaticallySetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "archive-test-runs-automatically-enabled";
            Metadata.Description = "archive-test-runs-automatically-enabled-description";
            Metadata.Section.Name = "archiving";
            Metadata.Section.Icon = SettingIcons.Archiving;
            Metadata.Category.Name = "test-execution";
            Metadata.Category.Icon = SettingIcons.Testing;
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

            var preferences = await _settingsProvider.GetDomainSettingsAsync<ArchiveSettings>(tenantId, context.ProjectId);
            preferences ??= new();
            return new FieldValue { BooleanValue = preferences.ArchiveTestRunsAutomatically, FieldDefinitionId = 0 };
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

            var tenantId = principal.GetTenantIdOrThrow();
            var preferences = await _settingsProvider.GetDomainSettingsAsync<ArchiveSettings>(tenantId, context.ProjectId);
            preferences ??= new();
            preferences.ArchiveTestRunsAutomatically = value.BooleanValue.Value;
            await _settingsProvider.SaveDomainSettingsAsync(tenantId, context.ProjectId, preferences);
        }
    }
}
