using Microsoft.Extensions.Localization;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestRepository.Services;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRepository.Commands;

/// <summary>
/// Deletesd a test suite folder and all child folders and test cases.
/// </summary>
internal class DeleteTestRepositoryFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestRepositoryController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public int SortOrder => 95;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;
    public bool Enabled => _appNavigationManager.State.SelectedTestRepositoryFolder is not null;
    public string Id => "delete-folder";
    public string Name => _loc["delete-folder"];
    public string Description => _loc["delete-test-repository-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestRepositoryFolder"];

    public DeleteTestRepositoryFolderCommand(AppNavigationManager appNavigationManager, TestRepositoryController controller, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = controller;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var folder = _appNavigationManager.State.SelectedTestRepositoryFolder;
        if (folder is null)
        {
            return;
        }
        await _controller.DeleteFolderByIdAsync(folder.Id);
    }
}
