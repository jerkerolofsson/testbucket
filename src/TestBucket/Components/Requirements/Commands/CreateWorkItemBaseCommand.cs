using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands;

internal abstract class CreateWorkItemBaseCommand : ICommand
{
    public string Id => $"create-{_type}";

    public string Name => _loc[$"create-{_type}"];

    public string Description => _loc[$"create-{_type}-description"];

    public int SortOrder => 50;

    public string? Folder => _loc["add"];

    public bool Enabled =>
        (
        _appNav.State.SelectedRequirement is not null ||
        _appNav.State.SelectedRequirementSpecificationFolder is not null ||
        _appNav.State.SelectedRequirementSpecification is not null
        ) &&
        _appNav.State.SelectedProject is not null;

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public virtual string? Icon => TbIcons.BoldDuoTone.Epic;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public string[] ContextMenuTypes => ["RequirementSpecification", "RequirementFolder", "Requirement", "menu-new"];

    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly RequirementBrowser _browser;
    private readonly IDialogService _dialogService;
    private readonly string _type;

    public CreateWorkItemBaseCommand(
        string type,
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService,
        RequirementBrowser browser)
    {
        _type = type;
        _loc = loc;
        _appNav = appNav;
        _requirementEditor = requirementEditor;
        _dialogService = dialogService;
        _browser = browser;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (!Enabled)
        {
            return;
        }

        var parent = _appNav.State.SelectedRequirement;
        var folder = _appNav.State.SelectedRequirementSpecificationFolder;
        var collection = _appNav.State.SelectedRequirementSpecification;

        if (folder is null && parent?.RequirementSpecificationFolderId is not null)
        {
            folder = await _browser.GetRequirementFolderByIdAsync(parent.RequirementSpecificationFolderId.Value);
        }
        if (collection is null && folder is not null)
        {
            collection = await _browser.GetRequirementSpecificationByIdAsync(folder.RequirementSpecificationId);
        }
        if (collection is null && parent is not null)
        {
            collection = await _browser.GetRequirementSpecificationByIdAsync(parent.RequirementSpecificationId);
        }

        var specificationId = collection?.Id;
        var folderId = folder?.Id;

        if (specificationId is null)
        {
            return;
        }

        var parameters = new DialogParameters<CreateRequirementDialog>
        {
            { x => x.RequirementType, _type }
        };
        var dialog = await _dialogService.ShowAsync<CreateRequirementDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Requirement requirement)
        {
            requirement.TestProjectId = _appNav.State.SelectedProject?.Id;
            requirement.TeamId = _appNav.State.SelectedProject?.TeamId;
            requirement.RequirementSpecificationId = specificationId.Value;
            requirement.RequirementSpecificationFolderId = folderId;
            requirement.ParentRequirementId = parent?.Id;
            if(parent is not null)
            {
                requirement.Progress = 0;
                requirement.StartDate = parent.StartDate;
                requirement.DueDate = parent.DueDate;
            }

            await _requirementEditor.AddRequirementAsync(requirement);
        }
    }
}
