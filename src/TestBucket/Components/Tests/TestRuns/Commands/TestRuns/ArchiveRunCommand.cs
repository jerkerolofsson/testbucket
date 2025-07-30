using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.Commands.TestRuns;

/// <summary>
/// Command to archive the run
/// </summary>
internal class ArchiveRunCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public int SortOrder => 95;
    public string? Folder => null;
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Write;
    public bool Enabled => _appNavigationManager.State.SelectedTestRun is not null && 
        (_appNavigationManager.State.SelectedTestRun.Archived == false);
    public string Id => "archive-test-run";
    public string Name => _loc["archive-test-run"];
    public string Description => _loc["archive-test-run-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Archive;
    public string[] ContextMenuTypes => ["TestRun"];
    public ArchiveRunCommand(AppNavigationManager appNavigationManager, TestCaseEditorController browser, IStringLocalizer<SharedStrings> loc)
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
        if (_appNavigationManager.State.SelectedTestRun.Archived == true)
        {
            return;
        }
        await _controller.ArchiveTestRunAsync(_appNavigationManager.State.SelectedTestRun);
    }
}
