using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

using static MudBlazor.CategoryTypes;

namespace TestBucket.Components.Requirements.Commands;

internal class CreateSpecificationCommand : ICommand
{
    public string Id => "create-requirement-specification";

    public string Name => _reqLoc["create-requirement-specification"];

    public string Description => _reqLoc["create-requirement-specification-description"];

    public bool Enabled => _appNav.State.SelectedProject is not null;

    public int SortOrder => 50;

    public string? Folder => null;

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Epic;
    public string[] ContextMenuTypes => ["menu-requirements", "menu-new"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.RequirementSpecification;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<RequirementStrings> _reqLoc;
    private readonly AppNavigationManager _appNav;
    private readonly RequirementEditorController _requirementEditor;
    private readonly IDialogService _dialogService;

    public CreateSpecificationCommand(
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
        var project = _appNav.State.SelectedProject;
        if(project is null)
        {
            return;
        }

        var spec = new RequirementSpecification()
        {
            Name = "New Specification",
            TenantId = project?.TenantId ?? "",
            TestProjectId = project?.Id,
            TeamId = project?.TeamId,
        };

        await _requirementEditor.AddRequirementSpecificationAsync(spec);
        _appNav.NavigateTo(spec);
    }
}
