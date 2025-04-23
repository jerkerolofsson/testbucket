using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestRuns.Dialogs;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Testing.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class AddTestSuiteToRunCommand : ICommand
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly TestRunCreationController _testRunCreationController;
    private readonly IDialogService _dialogService;

    public AddTestSuiteToRunCommand(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager,
        TestBrowser browser,
        TestRunCreationController testRunCreationController,
        IDialogService dialogService)
    {
        _loc = loc;
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _testRunCreationController = testRunCreationController;
        _dialogService = dialogService;
    }

    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null;
    public string Id => "add-test-suite-to-run";
    public string Name => _loc["add-to-run"];
    public string Description => "Adds test cases to a test run";
    public string? Icon => Icons.Material.Filled.PlaylistAdd;
    public string[] ContextMenuTypes => ["TestSuite"];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.Execute;

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


        var parameters = new DialogParameters<PickTestRunDialog>()
        {
            { x => x.TestProjectId, suite.TestProjectId }
        };
        var dialog = await _dialogService.ShowAsync<PickTestRunDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestRun testRun)
        {
            long[] testCaseIds = await _browser.GetTestSuiteSuiteTestsAsync(suite, excludeAutomated: true);
            foreach (var testCaseId in testCaseIds)
            {
                await _testRunCreationController.AddTestCaseToRunAsync(testRun, testCaseId);
            }

            if (!string.IsNullOrEmpty(suite.CiCdSystem))
            {
                await _testRunCreationController.StartAutomationAsync(testRun, suite.Variables, suite.Id);
            }

            _appNavigationManager.NavigateTo(testRun);
        }
    }
}
