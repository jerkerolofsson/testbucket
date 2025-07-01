using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Folders;

internal class NewRequirementSpecificationSearchFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly RequirementEditorController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public NewRequirementSpecificationSearchFolderCommand(AppNavigationManager appNavigationManager, RequirementEditorController controller, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = controller;
        _loc = loc;
    }

    public int SortOrder => 10;

    public string? Folder => _loc["add"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.RequirementSpecification;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedRequirementSpecification is not null;
    public string Id => "new-requirement-search-folder";
    public string Name => _loc["new-search-folder"];
    public string Description => _loc["new-folder-search-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.CreateNewFolder;
    public string[] ContextMenuTypes => ["RequirementSpecification"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var specification = _appNavigationManager.State.SelectedRequirementSpecification;
        if (specification is null)
        {
            return;
        }
        await _controller.AddSearchFolderAsync(specification);
    }
}
