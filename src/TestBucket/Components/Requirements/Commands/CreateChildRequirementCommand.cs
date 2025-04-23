
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Controls;
using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateChildRequirementCommand : ICommand
{
    public string Id => "create-child-requirement";

    public string Name => _loc["create-child-requirement"];

    public string Description => _loc["create-child-requirement-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.Filled.PaperPlane;

    public string[] ContextMenuTypes => ["Requirement"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly IDialogService _dialogService;

    public CreateChildRequirementCommand(
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService)
    {
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
        _dialogService = dialogService;
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
