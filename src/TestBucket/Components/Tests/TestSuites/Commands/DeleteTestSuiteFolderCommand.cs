using Microsoft.Extensions.Localization;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

/// <summary>
/// Deletesd a test suite folder and all child folders and test cases.
/// </summary>
internal class DeleteTestSuiteFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestSuiteController _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 95;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuiteFolder is not null;
    public string Id => "delete-test-suite-folder";
    public string Name => _loc["delete-folder"];
    public string Description => _loc["delete-test-suite-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestSuiteFolder"];

    public DeleteTestSuiteFolderCommand(AppNavigationManager appNavigationManager, TestSuiteController browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

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
