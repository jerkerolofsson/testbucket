using TestBucket.Domain.Editor.Models;

namespace TestBucket.Domain.Testing.TestCases.Features.ChangeStateToOngoingWhenEditingTests;
internal class ChangeStateToOngoingWhenEditingTestsSetting : SettingAdapter
{
    private readonly ISettingsProvider _settingsProvider;

    public ChangeStateToOngoingWhenEditingTestsSetting(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;

        Metadata.Name = "change-state-to-ongoing-when-editing-tests";
        Metadata.Description = "change-state-to-ongoing-when-editing-tests-description";
        Metadata.Category.Name = "testing";
        Metadata.Category.Icon = SettingIcons.Testing;
        Metadata.Section.Name = "editor";
        Metadata.Section.Icon = SettingIcons.Editor;
        Metadata.SearchText = "editor state";
        Metadata.ShowDescription = true;
        Metadata.Type = FieldType.Boolean;
        Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
    }

    public override async Task<FieldValue> ReadAsync(SettingContext context)
    {
        context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        var settings = await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
        settings ??= new();

        return new FieldValue { BooleanValue = settings.ChangeStateToOngoingWhenEditingTests, FieldDefinitionId = 0 };
    }

    public override async Task WriteAsync(SettingContext context, FieldValue value)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
        settings ??= new();

        if (settings.ChangeStateToOngoingWhenEditingTests != value.BooleanValue)
        {
            settings.ChangeStateToOngoingWhenEditingTests = value.BooleanValue ?? false;
            await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
        }
    }
}
