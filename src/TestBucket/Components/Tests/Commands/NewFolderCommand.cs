using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Tests.Commands;

internal class NewFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;

    public NewFolderCommand(AppNavigationManager appNavigationManager, TestBrowser browser)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
    }

    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null;
    public string Id => "new-folder";
    public string Name => "New Folder";
    public string Description => "Create a new folder";
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = "new-folder", Key = "F7", ModifierKeys = ModifierKey.None };
    public string? Icon => Icons.Material.Filled.CreateNewFolder;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder"];

    public async ValueTask ExecuteAsync()
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
