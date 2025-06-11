using Microsoft.Extensions.Localization;

using NGitLab.Models;

using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Models;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Testing.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestCases.Commands;

internal class RunTestCaseCommand : ICommand
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly TestRunCreationController _testRunCreationController;

    public RunTestCaseCommand(
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

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.Execute;
    public bool Enabled => _appNavigationManager.State.SelectedTestCase is not null ||
        _appNavigationManager.State.MultiSelectedTestCases.Count > 0;
    public string Id => "run-test-case";
    public string Name
    {
        get
        {
            if (_appNavigationManager.State.MultiSelectedTestCases.Count >= 2)
            {
                return string.Format(_loc["run-n-tests"], _appNavigationManager.State.MultiSelectedTestCases.Count);
            }

            return _loc["run"];
        }
    }
    public string Description => _loc["run-test-case-description"];
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = Id, Key = "KeyR", ModifierKeys = ModifierKey.Ctrl };
    public string? Icon => Icons.Material.Filled.PlayArrow;
    public string[] ContextMenuTypes => ["TestCase"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNavigationManager.State.MultiSelectedTestCases.Count > 0)
        {
            var testCase = _appNavigationManager.State.MultiSelectedTestCases.First();
            var projectId = testCase.TestProjectId;
            if (projectId is null)
            {
                return;
            }

            var testCaseIds = _appNavigationManager.State.MultiSelectedTestCases.Select(x => x.Id).ToArray();
            var names = _appNavigationManager.State.MultiSelectedTestCases.Take(3).Select(x => x.Name).ToArray();
            var name = string.Join(",", names);

            var run = await _testRunCreationController.CreateTestRunAsync(name, projectId.Value, testCaseIds);
            if (run is not null)
            {
                _appNavigationManager.NavigateTo(run);
            }
        }
        else if (_appNavigationManager.State.SelectedTestCase is { })
        {
            var testCase = _appNavigationManager.State.SelectedTestCase;
            var projectId = testCase.TestProjectId;
            if (projectId is null)
            {
                return;
            }
            var run = await _testRunCreationController.CreateTestRunAsync(testCase.Name, projectId.Value, [testCase.Id]);
            if (run is not null)
            {
                _appNavigationManager.NavigateTo(run);
            }

        }
    }
}
