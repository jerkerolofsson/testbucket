using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class NewFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public NewFolderCommand(AppNavigationManager appNavigationManager, TestBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public int SortOrder => 10;
    public string? Folder => _loc["add"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null;
    public string Id => "new-folder";
    public string Name => _loc["new-folder"];
    public string Description => _loc["new-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = Id, Key = "F7", ModifierKeys = ModifierKey.None };
    public string? Icon => Icons.Material.Filled.CreateNewFolder;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var suite = _appNavigationManager.State.SelectedTestSuite;
        if (suite is null)
        {
            return;
        }
        var projectId = suite.TestProjectId;
        if (projectId is null)
        {
            return;
        }
        await _browser.AddTestSuiteFolderAsync(projectId.Value, suite.Id, _appNavigationManager.State.SelectedTestSuiteFolder?.Id);
    }
}
