using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Models;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
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
    public bool Enabled => _appNavigationManager.State.SelectedTestCase is not null;
    public string Id => "run-test-case";
    public string Name => _loc["run"];
    public string Description => _loc["run-test-case-description"];
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = Id, Key = "KeyR", ModifierKeys = ModifierKey.Ctrl };
    public string? Icon => Icons.Material.Filled.PlayArrow;
    public string[] ContextMenuTypes => ["TestCase"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var testCase = _appNavigationManager.State.SelectedTestCase;
        if (testCase is null)
        {
            return;
        }
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
