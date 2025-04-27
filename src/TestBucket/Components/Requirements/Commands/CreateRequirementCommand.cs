using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateRequirementCommand : ICommand
{
    public string Id => "create-requirement";

    public string Name => _loc["create-requirement"];

    public string Description => _loc["create-requirement-description"];

    public int SortOrder => 50;

    public string? Folder => _loc["add"];

    public bool Enabled => 
        _appNav.State.SelectedRequirementSpecification is not null && 
        _appNav.State.SelectedProject is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.BoldDuoTone.Medal;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public string[] ContextMenuTypes => ["RequirementSpecification", "RequirementFolder"];

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly RequirementBrowser _browser;
    private readonly IDialogService _dialogService;

    public CreateRequirementCommand(
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService,
        RequirementBrowser browser)
    {
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
        _dialogService = dialogService;
        _browser = browser;
    }

    public async ValueTask ExecuteAsync()
    {
        if (!Enabled)
        {
            return;
        }

        var specificationId = _appNav.State.SelectedRequirementSpecification?.Id;
        var folderId = _appNav.State.SelectedRequirementSpecificationFolder?.Id;
        if(specificationId is null)
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
            requirement.RequirementSpecificationId = specificationId.Value;
            requirement.RequirementSpecificationFolderId = folderId;

            await _requirementEditor.AddRequirementAsync(requirement);
        }
    }
}
