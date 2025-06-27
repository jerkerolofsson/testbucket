
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Shared;
internal class EditRequirementCommand : ICommand
{
    public string Id => "edit-requirement";

    public string Name => _loc.Shared["edit"];

    public string Description => _loc.Requirements["edit-requirement-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null && _appNav.State.SelectedProject is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => Icons.Material.Filled.Edit;

    public string[] ContextMenuTypes => ["Requirement"];

    public int SortOrder => 1;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Write;

    private readonly IAppLocalization _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;

    public EditRequirementCommand(
        IAppLocalization loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor)
    {
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNav.State.SelectedRequirement is null)
        {
            return;
        }
        await _requirementEditor.OpenEditDialogAsync(_appNav.State.SelectedRequirement);
    }
}
