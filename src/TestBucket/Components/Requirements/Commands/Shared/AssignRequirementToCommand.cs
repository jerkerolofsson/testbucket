using TestBucket.Components.Requirements.Services;
using TestBucket.Contracts.Localization;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Requirements.Commands.Shared;
internal class AssignRequirementToCommand : ICommand
{
    public string Id => "assign-requirement-to";

    public string Name => _loc.Shared["assign-to"];

    public string Description => "";

    public bool Enabled => _appNav.State.MultiSelectedRequirements.Count > 0 || _appNav.State.SelectedRequirement is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.BoldDuoTone.UserCircle;

    public string[] ContextMenuTypes => ["Requirement"];

    public int SortOrder => 5;

    public string? Folder => _loc.Shared["assign"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Write;

    private readonly IAppLocalization _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;

    public AssignRequirementToCommand(
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
        if (_appNav.State.MultiSelectedRequirements.Count > 0)
        {
            foreach (var requirement in _appNav.State.MultiSelectedRequirements)
            {
                await _requirementEditor.AssignRequirementToAsync(requirement);
            }
        }
        else
        {
            var requirement = _appNav.State.SelectedRequirement;
            if (requirement is null)
            {
                return;
            }
            await _requirementEditor.AssignRequirementToAsync(requirement);
        }
    }
}
