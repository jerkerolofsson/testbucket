using TestBucket.Contracts.Localization;
using TestBucket.Domain.Testing.Settings;

namespace TestBucket.Domain.Testing.Features.DefaultMilestones;

internal class AddDefaultMilestoneToRunSetting : SettingAdapter
{
    private readonly ISettingsProvider _settingsProvider;

    public AddDefaultMilestoneToRunSetting(ISettingsProvider settingsProvider, IAppLocalization loc)
    {
        _settingsProvider = settingsProvider;

        Metadata.Name = loc.Shared["add-default-milestone-to-imported-run"];
        Metadata.Description = loc.Shared["add-default-milestone-to-imported-run-description"];
        Metadata.Category.Name = "testing";
        Metadata.Category.Icon = SettingIcons.Testing;
        Metadata.Section.Name = "import";
        Metadata.Section.Icon = SettingIcons.Import;
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

        var preferences = await _settingsProvider.GetDomainSettingsAsync<ImportTestsProjectSettings>(tenantId, context.ProjectId);
        preferences ??= new ImportTestsProjectSettings() { };
        return new FieldValue { BooleanValue = preferences.AddDefaultMilestoneFieldToImportedTestRuns, FieldDefinitionId = 0 };
    }

    public override async Task WriteAsync(SettingContext context, FieldValue value)
    {
        var principal = context.Principal;
        var tenantId = principal.GetTenantIdOrThrow();
        if (principal.Identity?.Name is null)
        {
            return;
        }

        if (value.BooleanValue is null)
        {
            return;
        }

        var preferences = await _settingsProvider.GetDomainSettingsAsync<ImportTestsProjectSettings>(tenantId, context.ProjectId);
        preferences ??= new ImportTestsProjectSettings() { };
        preferences.AddDefaultMilestoneFieldToImportedTestRuns = value.BooleanValue.Value;
        await _settingsProvider.SaveDomainSettingsAsync(tenantId, context.ProjectId, preferences);
    }
}
