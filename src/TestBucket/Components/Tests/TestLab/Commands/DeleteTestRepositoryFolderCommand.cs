using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestLab.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestLab.Commands;

/// <summary>
/// Deletesd a test suite folder and all child folders and test cases.
/// </summary>
internal class DeleteTestLabFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestLabController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public int SortOrder => 95;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;
    public bool Enabled => _appNavigationManager.State.SelectedTestLabFolder is not null;
    public string Id => "delete-folder";
    public string Name => _loc["delete-folder"];
    public string Description => _loc["delete-test-lab-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestLabFolder"];

    public DeleteTestLabFolderCommand(AppNavigationManager appNavigationManager, TestLabController controller, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = controller;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var folder = _appNavigationManager.State.SelectedTestLabFolder;
        if (folder is null)
        {
            return;
        }
        await _controller.DeleteFolderByIdAsync(folder.Id);
    }
}
