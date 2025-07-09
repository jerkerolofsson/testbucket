using TestBucket.Contracts.Localization;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Requirements.Commands.Collections;

internal class ExportCollectionCommand : ICommand
{
    public string Id => "export-collection";

    public string Name => _loc.Requirements["export-collection"];

    public string Description => _loc.Requirements["export-collection-description"];

    public bool Enabled => _appNav.State.SelectedRequirementSpecification is not null;
    public int SortOrder => 70;
    public string? Folder => null;

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Import;
    public string[] ContextMenuTypes => ["menu-requirements", "RequirementSpecification"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.RequirementSpecification;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    private readonly IAppLocalization _loc;
    private readonly AppNavigationManager _appNav;

    public ExportCollectionCommand(
        AppNavigationManager appNav,
        IAppLocalization loc)
    {
        _appNav = appNav;
        _loc = loc;
    }

    public ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var collection = _appNav.State.SelectedRequirementSpecification;
        if(collection is null)
        {
            return ValueTask.CompletedTask;
        }

        _appNav.NavigateTo(_appNav.GetExportSpecificationsUrl(collection), true);
        return ValueTask.CompletedTask;
    }
}
