using Microsoft.Extensions.Localization;

using TestBucket.Components.Issues.Controllers;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Issues.Commands;
internal class DeleteIssueCommand : Domain.Commands.ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNav;
    private readonly IssueController _issueController;

    public int SortOrder => 80;

    public string? Folder => null;

    public bool Enabled => _appNav.State.SelectedIssue is not null;
    public string Id => "delete-issue";
    public string Name => _loc["delete-issue"];
    public string Description => "";
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["issue"];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Issue;
    public PermissionLevel? RequiredLevel => PermissionLevel.Delete;

    public DeleteIssueCommand(
        IStringLocalizer<IssueStrings> loc,
        AppNavigationManager appNav,
        IssueController issueController)
    {
        _loc = loc;
        _appNav = appNav;
        _issueController = issueController;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var issue = _appNav.State.SelectedIssue;
        if (issue is null)
        {
            return;
        }
        await _issueController.DeleteLocalIssueAsync(issue);
    }
}
