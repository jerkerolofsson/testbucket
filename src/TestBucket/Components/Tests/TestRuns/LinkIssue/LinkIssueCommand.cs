using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Components.Tests.TestRuns.LinkIssue;

public class LinkIssueCommand : ICommand
{
    private readonly IIssueManager _issueManager;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IDialogService _dialogService;

    public LinkIssueCommand(
        IIssueManager issueManager,
        AuthenticationStateProvider authStateProvider,
        AppNavigationManager appNavigationManager, IDialogService dialogService)
    {
        _issueManager = issueManager;
        _authStateProvider = authStateProvider;
        _appNavigationManager = appNavigationManager;
        _dialogService = dialogService;
    }
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "link-issue";
    public string Name => "Link Issue";
    public string Description => "Links an issue to a test case run";
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = "link-issue", Key = "KeyI", ModifierKeys = ModifierKey.Ctrl };
    public string? Icon => Icons.Material.Outlined.AddLink;
    public string[] ContextMenuTypes => ["TestCaseRun"];

    public async ValueTask ExecuteAsync()
    {
        var testCaseRun = _appNavigationManager.State.SelectedTestCaseRun;
        if (testCaseRun?.TestProjectId is null)
        {
            return;
        }

        var parameters = new DialogParameters<LinkIssueDialog>()
        {
            { x => x.TestProjectId, testCaseRun.TestProjectId.Value }
        };
        var dialog = await _dialogService.ShowAsync<LinkIssueDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;

        if(result?.Data is IssueDto issue)
        {
            var linkedIssue = new LinkedIssue
            {
                Title = issue.Title,
                Description = issue.Description,
                State = issue.State,

                ExternalId = issue.ExternalId,
                ExternalSystemId = issue.ExternalSystemId,
                ExternalSystemName = issue.ExternalSystemName,
                ExternalDisplayId = issue.ExternalDisplayId,
                IssueType = issue.IssueType,
                MilestoneName = issue.MilestoneName,
                Url = issue.Url,
                Author = issue.Author,

                Created = issue.Created ?? DateTimeOffset.UtcNow,
                Modified = issue.Modified ?? DateTimeOffset.UtcNow,

                TestProjectId = testCaseRun.TestProjectId,
                TestCaseRunId = testCaseRun.Id,
                TeamId = testCaseRun.TeamId,
                TenantId = testCaseRun.TenantId,
            };

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            await _issueManager.AddLinkedIssueAsync(authState.User, linkedIssue);
        }
    }
}
