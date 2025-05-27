using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Domain.Issues.Commands;
internal class CloseIssueCommand : ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNav;
    private readonly IIssueManager _issueManager;

    public int SortOrder => 20;

    public string? Folder => null;

    public bool Enabled => true;
    public string Id => "close-issue";
    public string Name => _loc["close-issue"];
    public string Description => "";
    public string? Icon => TbIcons.BoldDuoTone.CloudDownload;
    public string[] ContextMenuTypes => [];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Issue;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;

    public CloseIssueCommand(
        IStringLocalizer<SettingStrings> loc,
        AppNavigationManager appNav,
        IIssueManager issueManager)
    {
        _loc = loc;
        _appNav = appNav;
        _issueManager = issueManager;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNav.State.SelectedIssue is null)
        {
            return;
        }

        var issue = _appNav.State.SelectedIssue;
        issue.MappedState = Contracts.Issues.States.MappedIssueState.Closed;
        issue.State = IssueStates.Closed;
        await _issueManager.UpdateLocalIssueAsync(principal, issue);
    }
}
