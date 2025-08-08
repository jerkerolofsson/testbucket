using Microsoft.Extensions.Localization;

using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Shared;
internal class ApproveRequirementCommand : ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNav;
    private readonly IRequirementManager _manager;

    public int SortOrder => 20;

    public string? Folder => null;

    public bool Enabled => _appNav.State.SelectedRequirement is not null;
    public string Id => "approve-requirement";
    public string Name {
        get 
        {
            if(_appNav.State.SelectedRequirement is null)
            {
                return "";
            }
            var message = _loc["approve-requirement-arg"];
            return string.Format(message, _appNav.State.SelectedRequirement.Name);
        }
    }
    public string Description => "";
    public string? Icon => TbIcons.BoldDuoTone.VerifiedCheck;
    public string[] ContextMenuTypes => [];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Approve;

    public ApproveRequirementCommand(
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        IRequirementManager manager)
    {
        _loc = loc;
        _appNav = appNav;
        _manager = manager;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNav.State.SelectedRequirement is null)
        {
            return;
        }

        var requirement = _appNav.State.SelectedRequirement;
        await _manager.ApproveRequirementAsync(principal, requirement);
    }
}
