
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Collections;
internal class SplitSpecificationCommand : ICommand
{
    public string Id => "split-requirement-specification";

    public string Name => _loc["split-requirement-specification"];

    public string Description => _loc["split-requirement-specification-description"];

    public bool Enabled => _appNav.State.SelectedRequirementSpecification is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => Icons.Material.Filled.CallSplit;

    public string[] ContextMenuTypes => ["RequirementSpecification"];

    public int SortOrder => 80;

    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Write;

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;

    public SplitSpecificationCommand(
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
        await _requirementEditor.ExtractRequirementsFromSpecificationAsync(_appNav.State.SelectedRequirementSpecification);
    }
}
