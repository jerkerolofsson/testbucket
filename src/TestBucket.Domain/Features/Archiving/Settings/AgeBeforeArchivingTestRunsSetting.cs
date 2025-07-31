using TestBucket.Domain.Features.Archiving.Models;

namespace TestBucket.Domain.Features.Archiving.Settings
{
    public class AgeBeforeArchivingTestRunsSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AgeBeforeArchivingTestRunsSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "archive-test-runs-automatically-age";
            Metadata.Description = "archive-test-runs-automatically-age-description";
            Metadata.Section.Name = "archiving";
            Metadata.Section.Icon = SettingIcons.Archiving;
            Metadata.Category.Name = "testing";
            Metadata.Category.Icon = SettingIcons.Testing;
            Metadata.Type = FieldType.TimeSpan;
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
            return new FieldValue { TimeSpanValue = preferences.AgeBeforeArchivingTestRuns, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var principal = context.Principal;
            if (principal.Identity?.Name is null)
            {
                return;
            }

            if (value.TimeSpanValue is null)
            {
                return;
            }

            var tenantId = principal.GetTenantIdOrThrow();
            var preferences = await _settingsProvider.GetDomainSettingsAsync<ArchiveSettings>(tenantId, context.ProjectId);
            preferences ??= new();
            preferences.AgeBeforeArchivingTestRuns = value.TimeSpanValue.Value;
            await _settingsProvider.SaveDomainSettingsAsync(tenantId, context.ProjectId, preferences);
        }
    }
}
