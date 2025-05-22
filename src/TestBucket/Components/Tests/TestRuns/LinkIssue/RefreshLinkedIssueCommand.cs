using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestRuns.LinkIssue;

public class RefreshLinkedIssueCommand : ICommand
{
    private readonly IIssueManager _issueManager;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<IssueStrings> _loc;

    public RefreshLinkedIssueCommand(
        IIssueManager issueManager,
        AuthenticationStateProvider authStateProvider,
        AppNavigationManager appNavigationManager, IDialogService dialogService, IStringLocalizer<IssueStrings> loc)
    {
        _issueManager = issueManager;
        _authStateProvider = authStateProvider;
        _appNavigationManager = appNavigationManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    public int SortOrder => 10;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCaseRun;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "refresh-linked-issue";
    public string Name => _loc[Id];
    public string Description => "Updates an issue from external system";
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Outlined.Refresh;
    public string[] ContextMenuTypes => ["LinkedIssue"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var linkedIssue = _appNavigationManager.State.SelectedLinkedIssue;
        if (linkedIssue is null)
        {
            return;
        }

        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        await _issueManager.RefreshLinkedIssueAsync(authState.User, linkedIssue);
    }
}
