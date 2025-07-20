using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Controllers;
using TestBucket.Domain;
using TestBucket.Domain.AI.Runner;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing.TestRuns.Search;
using TestBucket.Localization;

namespace TestBucket.Components.AI.Runner.Commands;

internal class AssignAllUnassignedToAiRunner : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IProgressManager _progressManager;
    private readonly TestCaseEditorController _controller;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 5;

    public string? Folder => null;

    public AssignAllUnassignedToAiRunner(
        AppNavigationManager appNavigationManager, 
        IProgressManager progressManager,
        TestCaseEditorController controller, 
        IStringLocalizer<SharedStrings> loc, 
        TestBrowser browser)
    {
        _appNavigationManager = appNavigationManager;
        _progressManager = progressManager;
        _controller = controller;
        _loc = loc;
        _browser = browser;
    }

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.Read;
    public bool Enabled => _appNavigationManager.State.SelectedTestRun is not null;
    public string Id => "assign-all-unassigned-to-ai";
    public string Name => _loc["assign-all-unassigned-to-ai"];
    public string Description => _loc["assign-all-unassigned-to-ai-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.UserCircle;
    public string[] ContextMenuTypes => ["TestRun"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var run = _appNavigationManager.State.SelectedTestRun;
        if (run is null)
        {
            return;
        }

        int offset = 0;
        int count = 10;
        await using var task = _progressManager.CreateProgressTask(_loc["assigning-tests"]);
        while (true)
        {
            var result = await _browser.SearchTestCaseRunsAsync(new SearchTestCaseRunQuery 
            { 
                Unassigned = true,
                TestRunId = run.Id, 
                Count = count, 
                Offset = 0
            });
            offset += result.Items.Length;

            if(result.TotalCount == 0)
            {
                break;
            }

            foreach(var testCaseRun in result.Items)
            {
                testCaseRun.AssignedToUserName = AiRunnerConstants.Username;
                await _controller.SaveTestCaseRunAsync(testCaseRun, informObservers:false);
            }

            double percent = offset * 100.0 / result.TotalCount;
            await task.ReportStatusAsync(_loc["assigning-tests"], percent);
        }
    }
}
