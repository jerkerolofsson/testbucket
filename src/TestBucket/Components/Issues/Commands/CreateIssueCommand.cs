using Microsoft.Extensions.Localization;

using TestBucket.Components.Issues.Controllers;
using TestBucket.Components.Issues.Dialogs;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Issues.Commands;

internal class CreateIssueCommand : ICommand
{
    public string Id => "create-issue";

    public string Name => _issueLoc["create-issue"];

    public string Description => _issueLoc["create-issue-description"];

    public bool Enabled => _appNav.State.SelectedProject is not null;

    public int SortOrder => 50;

    public string? Folder => _loc["add"];

    public KeyboardBinding? DefaultKeyboardBinding => null;

    public string? Icon => TbIcons.BoldDuoTone.Bug;

    public string[] ContextMenuTypes => ["menu-new", "issue"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Issue;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<IssueStrings> _issueLoc;
    private readonly AppNavigationManager _appNav;
    private readonly IssueController _controller;
    private readonly IDialogService _dialogService;

    public CreateIssueCommand(
        IStringLocalizer<IssueStrings> issueLoc,
        AppNavigationManager appNav,
        IssueController controller,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc)
    {
        _issueLoc = issueLoc;
        _appNav = appNav;
        _controller = controller;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNav.State.SelectedProject is null)
        {
            return;
        }

        var parameters = new DialogParameters<CreateIssueDialog>
        {
            { x => x.TestProjectId, _appNav.State.SelectedProject.Id }
        };
        var dialog = await _dialogService.ShowAsync<CreateIssueDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is LocalIssue issue)
        {
            issue.TestProjectId = _appNav.State.SelectedProject.Id;
            issue.TeamId = _appNav.State.SelectedProject.TeamId;

            await _controller.AddIssueAsync(issue);
        }

    }
}
