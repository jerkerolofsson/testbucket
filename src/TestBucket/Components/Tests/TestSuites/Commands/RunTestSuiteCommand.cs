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

internal class RunTestSuiteCommand : ICommand
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly TestRunCreationController _testRunCreationController;

    public RunTestSuiteCommand(
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
        _appNavigationManager.State.SelectedTestSuite is not null &&
        _appNavigationManager.State.SelectedTestSuiteFolder is null &&
        _appNavigationManager.State.SelectedTestCase is null;
    public string Id => "run-test-suite";
    public string Name => _loc["run-test-suite"];
    public string Description => "Runs a test suite";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.PlayArrow;
    public string[] ContextMenuTypes => ["TestSuite"];

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
        long[] testCaseIds = await _browser.GetTestSuiteSuiteTestsAsync(suite, excludeAutomated: true);

        bool startAutomation = true;
        var run = await _testRunCreationController.CreateTestRunAsync(suite, testCaseIds, startAutomation);
        if (run is not null)
        {
            _appNavigationManager.NavigateTo(run);
        }
    }
}
