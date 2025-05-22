using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Models;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class RunTestSuiteFolderCommand : ICommand
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly TestRunCreationController _testRunCreationController;

    public RunTestSuiteFolderCommand(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager,
        TestBrowser browser,
        TestRunCreationController testRunCreationController)
    {
        _loc = loc;
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _testRunCreationController = testRunCreationController;
    }

    public int SortOrder => 10;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.Execute;
    public bool Enabled => 
        _appNavigationManager.State.SelectedTestSuiteFolder is not null &&
        _appNavigationManager.State.SelectedTestCase is null;
    public string Id => "run-folder";
    public string Name => _loc["run-folder"];
    public string Description => _loc["run-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.PlayArrow;
    public string[] ContextMenuTypes => ["TestSuiteFolder"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var folder = _appNavigationManager.State.SelectedTestSuiteFolder;
        if (folder is null)
        {
            return;
        }
        var projectId = folder.TestProjectId;
        if (projectId is null)
        {
            return;
        }
        long[] testCaseIds = await _browser.GetTestSuiteSuiteFolderTestsAsync(folder, true);

        var run = await _testRunCreationController.CreateTestRunAsync(folder.Name, projectId.Value, testCaseIds);
        if (run is not null)
        {
            _appNavigationManager.NavigateTo(run);
        }
    }
}
