
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Collections;
internal class DeleteRequirementSpecificationCommand : ICommand
{
    public string Id => "delete-requirement-specification";

    public string Name => _loc["delete-requirement-specification"];

    public string Description => _loc["delete-requirement-specification-description"];

    public bool Enabled => _appNav.State.SelectedRequirementSpecification is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => Icons.Material.Filled.Delete;

    public string[] ContextMenuTypes => ["RequirementSpecification"];

    public int SortOrder => 90;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.RequirementSpecification;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;

    public DeleteRequirementSpecificationCommand(
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor)
    {
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNav.State.SelectedRequirementSpecification is null)
        {
            return;
        }
        await _requirementEditor.DeleteRequirementSpecificationAsync(_appNav.State.SelectedRequirementSpecification);
    }
}
