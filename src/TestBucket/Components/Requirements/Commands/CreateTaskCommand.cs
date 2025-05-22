using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateTaskCommand : ICommand
{
    public string Id => "create-child-task";

    public string Name => _reqLoc["create-child-task"];

    public string Description => _reqLoc["create-child-task-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null;

    public int SortOrder => 30;

    public string? Folder => _loc["add"];

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.CheckList;
    public string[] ContextMenuTypes => ["Requirement"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<RequirementStrings> _reqLoc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly IDialogService _dialogService;

    public CreateTaskCommand(
        IStringLocalizer<RequirementStrings> reqLoc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc)
    {
        _reqLoc = reqLoc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNav.State.SelectedRequirement is null)
        {
            return;
        }

        var parameters = new DialogParameters<CreateRequirementDialog>
        {
            { x => x.RequirementType, RequirementTypes.Task }
        };
        var dialog = await _dialogService.ShowAsync<CreateRequirementDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Requirement requirement)
        {
            requirement.TestProjectId = _appNav.State.SelectedRequirement.TestProjectId;
            requirement.TeamId = _appNav.State.SelectedRequirement.TeamId;
            requirement.RequirementSpecificationId = _appNav.State.SelectedRequirement.RequirementSpecificationId;
            requirement.RequirementSpecificationFolderId = _appNav.State.SelectedRequirement.RequirementSpecificationFolderId;
            requirement.ParentRequirementId = _appNav.State.SelectedRequirement.Id;

            await _requirementEditor.AddRequirementAsync(requirement);
        }

    }
}
