
using Microsoft.Extensions.Localization;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;
internal class DeleteRequirementCommand : ICommand
{
    public string Id => "delete-requirement";

    public string Name => _loc["delete-requirement"];

    public string Description => _loc["delete-requirement-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null && _appNav.State.SelectedProject is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => Icons.Material.Filled.Delete;

    public string[] ContextMenuTypes => ["Requirement"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;

    public DeleteRequirementCommand(
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor)
    {
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
    }

    public async ValueTask ExecuteAsync()
    {
        if (_appNav.State.SelectedRequirement is null)
        {
            return;
        }
        await _requirementEditor.DeleteRequirementAsync(_appNav.State.SelectedRequirement);
    }
}
