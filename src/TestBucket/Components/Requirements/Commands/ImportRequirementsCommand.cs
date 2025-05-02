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

namespace TestBucket.Components.Requirements.Commands;

internal class ImportRequirementsCommand : ICommand
{
    public string Id => "import-requirements";

    public string Name => _reqLoc["import-requirements"];

    public string Description => _reqLoc["import-requirements-description"];

    public bool Enabled => _appNav.State.SelectedProject is not null;

    public int SortOrder => 50;

    public string? Folder => null;

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Outlined.ImportExport;
    public string[] ContextMenuTypes => ["menu-requirements", "menu-import"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.RequirementSpecification;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<RequirementStrings> _reqLoc;
    private readonly AppNavigationManager _appNav;

    public ImportRequirementsCommand(
        IStringLocalizer<RequirementStrings> reqLoc,
        AppNavigationManager appNav,
        IStringLocalizer<SharedStrings> loc)
    {
        _reqLoc = reqLoc;
        _appNav = appNav;
        _loc = loc;
    }

    public ValueTask ExecuteAsync()
    {
        var project = _appNav.State.SelectedProject;
        if(project is null)
        {
            return ValueTask.CompletedTask;
        }

        _appNav.NavigateTo(_appNav.GetImportSpecificationsUrl(), false);
        return ValueTask.CompletedTask;
    }
}
