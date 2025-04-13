using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Tests.TestRuns.LinkIssue;

public class LinkIssueCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IDialogService _dialogService;

    public LinkIssueCommand(AppNavigationManager appNavigationManager, IDialogService dialogService)
    {
        _appNavigationManager = appNavigationManager;
        _dialogService = dialogService;
    }

    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "link-issue";
    public string Name => "Link Issue";
    public string Description => "Links an issue to a test case run";
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = "link-issue", Key = "KeyI", ModifierKeys = ModifierKey.Ctrl | ModifierKey.Shift };
    public string? Icon => Icons.Material.Outlined.AddLink;
    public string[] ContextMenuTypes => ["TestCaseRun"];

    public async ValueTask ExecuteAsync()
    {
        if (_appNavigationManager.State.SelectedTestCaseRun?.TestProjectId is null)
        {
            return;
        }

        var parameters = new DialogParameters<LinkIssueDialog>()
        {
            { x => x.TestProjectId, _appNavigationManager.State.SelectedTestCaseRun.TestProjectId.Value }
        };
        var dialog = await _dialogService.ShowAsync<LinkIssueDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        await dialog.Result;

    }
}
