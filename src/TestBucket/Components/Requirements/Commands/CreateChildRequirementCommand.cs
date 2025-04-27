using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateChildRequirementCommand : ICommand
{
    public string Id => "create-child-requirement";

    public string Name => _reqLoc["create-child-requirement"];

    public string Description => _reqLoc["create-child-requirement-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null;

    public int SortOrder => 50;

    public string? Folder => _loc["add"];

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.BoldDuoTone.Medal;

    public string[] ContextMenuTypes => ["Requirement"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<RequirementStrings> _reqLoc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly IDialogService _dialogService;

    public CreateChildRequirementCommand(
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

    public async ValueTask ExecuteAsync()
    {
        if (_appNav.State.SelectedRequirement is null)
        {
            return;
        }

        var parameters = new DialogParameters<CreateRequirementDialog>
        {
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
