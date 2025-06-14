using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands;

/// <summary>
/// Navigates to the test run associated with the test case run
/// </summary>
internal class GoToTestRunCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly TestRunController _controller;

    public int SortOrder => 99;
    public string? Folder => null;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCaseRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedTestCaseRun is not null;
    public string Id => "go-to-run-from-test-case-run";
    public string Name => _loc["go-to-run"];
    public string Description => _loc["go-to-run-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Link;
    public string[] ContextMenuTypes => ["TestCaseRun"];
    public GoToTestRunCommand(AppNavigationManager appNavigationManager, IStringLocalizer<SharedStrings> loc, TestRunController controller)
    {
        _appNavigationManager = appNavigationManager;
        _loc = loc;
        _controller = controller;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedTestCaseRun is null)
        {
            return;
        }
        var testRun = _appNavigationManager.State.SelectedTestCaseRun.TestRun;
        if (testRun is null)
        {
            testRun = await _controller.GetTestRunByIdAsync(_appNavigationManager.State.SelectedTestCaseRun.TestRunId);
        }
        if (testRun is null)
        {
            return;
        }

        _appNavigationManager.NavigateTo(testRun);
        return;
    }
}
