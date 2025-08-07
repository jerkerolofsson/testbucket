using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands.TestRuns;

internal class RunFailedAgainCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestRunCreationController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public int SortOrder => 30;
    public string? Folder => null;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedTestRun is not null;
    public string Id => "run-failed-again";
    public string Name => _loc["run-failed-again"];
    public string Description => _loc["run-failed-again-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.ContentCopy;
    public string[] ContextMenuTypes => ["TestRun"];
    public RunFailedAgainCommand(AppNavigationManager appNavigationManager, TestRunCreationController browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = browser;
        _loc = loc;
    }
    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedTestRun is null)
        {
            return;
        }

        var run = await _controller.DuplicateTestRunAsync(_appNavigationManager.State.SelectedTestRun, "result:failed");
        _appNavigationManager.NavigateTo(run);
    }
}
