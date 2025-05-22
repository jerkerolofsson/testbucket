using Microsoft.Extensions.Localization;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class DeleteTestSuiteFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestSuiteService _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public DeleteTestSuiteFolderCommand(AppNavigationManager appNavigationManager, TestSuiteService browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public int SortOrder => 90;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuiteFolder is not null;
    public string Id => "delete-folder";
    public string Name => _loc["delete-folder"];
    public string Description => _loc["delete-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestSuiteFolder"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var folder = _appNavigationManager.State.SelectedTestSuiteFolder;
        if (folder is null)
        {
            return;
        }
        await _browser.DeleteFolderByIdAsync(folder.Id);
    }
}
