using TestBucket.Domain.Editor.Models;

namespace TestBucket.Domain.Testing.TestCases.Features.ChangeStateToCompletedWhenApproved;
internal class ChangeStateToCompletedWhenApprovedSetting : SettingAdapter
{
    private readonly ISettingsProvider _settingsProvider;

    public ChangeStateToCompletedWhenApprovedSetting(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;

        Metadata.Name = "change-state-to-completed-when-approved";
        Metadata.Description = "change-state-to-completed-when-approved-description";
        Metadata.Category.Name = "testing";
        Metadata.Category.Icon = SettingIcons.Testing;
        Metadata.Section.Name = "editor";
        Metadata.Section.Icon = SettingIcons.Editor;
        Metadata.SearchText = "editor state review approve";
        Metadata.ShowDescription = true;
        Metadata.Type = FieldType.Boolean;
        Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
    }

    public override async Task<FieldValue> ReadAsync(SettingContext context)
    {
        context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        var settings = await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
        settings ??= new();

        return new FieldValue { BooleanValue = settings.ChangeStateToCompletedWhenApproved, FieldDefinitionId = 0 };
    }

    public override async Task WriteAsync(SettingContext context, FieldValue value)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
        settings ??= new();

        if (settings.ChangeStateToCompletedWhenApproved != value.BooleanValue)
        {
            settings.ChangeStateToCompletedWhenApproved = value.BooleanValue ?? false;
            await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
        }
    }
}
