using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Controls;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Tests.TestCases.Commands;

internal class SyncWithActiveDocumentCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;

    public SyncWithActiveDocumentCommand(AppNavigationManager appNavigationManager, TestBrowser browser)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
    }

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedTestCase is not null;
    public string Id => "sync-with-active-doc";
    public string Name => "Sync with active document";
    public string Description => "";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.CompareArrows;
    public string[] ContextMenuTypes => ["TestCase"];

    public async ValueTask ExecuteAsync()
    {
        if (_appNavigationManager.State.SelectedTestCase is null)
        {
            return;
        }

        await _browser.SyncWithActiveDocumentAsync(_appNavigationManager.State.SelectedTestCase);
        //var testCase = await _controller.CreateNewTestCaseAsync(_appNavigationManager.State.SelectedProject, _appNavigationManager.State.SelectedTestSuiteFolder, _appNavigationManager.State.SelectedTestSuite?.Id);
    }
}
