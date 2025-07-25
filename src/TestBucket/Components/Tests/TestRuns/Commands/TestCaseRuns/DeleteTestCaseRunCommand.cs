using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands.TestCaseRuns;

/// <summary>
/// Navigates to the test run associated with the test case run
/// </summary>
internal class DeleteTestCaseRunCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly TestRunController _controller;

    public int SortOrder => 99;
    public string? Folder => null;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCaseRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;
    public bool Enabled => _appNavigationManager.State.SelectedTestCaseRun is not null;
    public string Id => "delete-test-case-run";
    public string Name
    {
        get
        {
            if (_appNavigationManager.State.SelectedTestCaseRun is not null)
            {
                return string.Format(_loc["delete-test-case-run-fmt"], _appNavigationManager.State.SelectedTestCaseRun.Name);
            }
            return "";
        }
    }
    public string Description => _loc["delete-test-case-run-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestCaseRun"];
    public DeleteTestCaseRunCommand(AppNavigationManager appNavigationManager, IStringLocalizer<SharedStrings> loc, TestRunController controller)
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
        await _controller.DeleteTestCaseRunAsync(_appNavigationManager.State.SelectedTestCaseRun);
    }
}
