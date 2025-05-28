using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Requests;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Issues.Commands;
internal class CloseIssueCommand : Domain.Commands.ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNav;
    private readonly IMediator _mediator;

    public int SortOrder => 20;

    public string? Folder => null;

    public bool Enabled => _appNav.State.SelectedIssue is not null;
    public string Id => "close-issue";
    public string Name => _loc["close-issue"];
    public string Description => "";
    public string? Icon => Icons.Material.Filled.Close;
    public string[] ContextMenuTypes => [];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Issue;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;

    public CloseIssueCommand(
        IStringLocalizer<SettingStrings> loc,
        AppNavigationManager appNav,
        IMediator mediator)
    {
        _loc = loc;
        _appNav = appNav;
        _mediator = mediator;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNav.State.SelectedIssue is null)
        {
            return;
        }

        var issue = _appNav.State.SelectedIssue;

        await _mediator.Send(new CloseIssueRequest(principal, issue));
    }
}
