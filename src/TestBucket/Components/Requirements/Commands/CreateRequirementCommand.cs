
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Controls;
using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Icons;
using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateRequirementCommand : ICommand
{
    public string Id => "create-requirement";

    public string Name => _loc["create-requirement"];

    public string Description => _loc["create-requirement-description"];

    public bool Enabled => _appNav.State.SelectedRequirement is not null && _appNav.State.SelectedProject is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.Filled.PaperPlane;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public string[] ContextMenuTypes => ["RequirementSpecification", "RequirementFolder"];

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly IDialogService _dialogService;

    public CreateRequirementCommand(
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
        if (_appNav.State.SelectedRequirement is null || _appNav.State.SelectedProject is null || _appNav.State.SelectedRequirementSpecification is null)
        {
            return;
        }

        var parameters = new DialogParameters<CreateRequirementDialog>
        {
        };
        var dialog = await _dialogService.ShowAsync<CreateRequirementDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if(result?.Data is Requirement requirement)
        {
            requirement.TestProjectId = _appNav.State.SelectedProject?.Id;
            requirement.TeamId = _appNav.State.SelectedProject?.TeamId;
            requirement.RequirementSpecificationId = _appNav.State.SelectedRequirementSpecification.Id;
            requirement.RequirementSpecificationFolderId = _appNav.State.SelectedRequirementSpecificationFolder?.Id;

            await _requirementEditor.AddRequirementAsync(requirement);
        }
    }
}
