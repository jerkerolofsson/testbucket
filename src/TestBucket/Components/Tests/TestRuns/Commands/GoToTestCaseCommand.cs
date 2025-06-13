using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands;

/// <summary>
/// Command to delete the run
/// </summary>
internal class GoToTestCaseCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 99;
    public string? Folder => null;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCaseRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedTestCaseRun is not null;
    public string Id => "go-to-test-case-from-test-case-run";
    public string Name => _loc["go-to-testcase"];
    public string Description => _loc["go-to-testcase-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Link;
    public string[] ContextMenuTypes => ["TestCaseRun"];
    public GoToTestCaseCommand(AppNavigationManager appNavigationManager, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _loc = loc;
    }


    public ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedTestCaseRun is null)
        {
            return ValueTask.CompletedTask;
        }
        var testCase = _appNavigationManager.State.SelectedTestCaseRun.TestCase;

        if(testCase is null)
        {
            return ValueTask.CompletedTask;
        }

        _appNavigationManager.NavigateTo(testCase);

        return ValueTask.CompletedTask;
    }
}
