using Microsoft.Extensions.Localization;

using TestBucket.Components.Issues.Controllers;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Issues.Integrations;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Projects;
using TestBucket.Domain.States;
using TestBucket.Localization;

namespace TestBucket.Components.Issues.Commands;

internal class LinkIssueWithIntegrationCommand : ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNav;
    private readonly IProjectManager _projectManager;
    private readonly IStateService _stateService;
    private readonly List<IExternalIssueProvider> _externalIssueProviders;
    private readonly IssueController _issueController;

    public int SortOrder => 5;

    public string? Folder => null;

    public bool Enabled => _appNav.State.SelectedIssue is not null && 
        _appNav.State.SelectedIssue.ExternalId is null;

    public string Id => "link-issue-with-jira";
    public string Name => _loc["link-issue-with-jira"];
    public string Description => "";
    public string? Icon => TbIcons.Brands.Jira;
    public string[] ContextMenuTypes => ["issue"];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Issue;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;

    public LinkIssueWithIntegrationCommand(
        IProjectManager projectManager,
        IEnumerable<IExternalIssueProvider> externalIssueProviders,
        AppNavigationManager appNav,
        IStringLocalizer<IssueStrings> loc,
        IssueController issueController,
        IStateService stateService)
    {
        _projectManager = projectManager;
        _externalIssueProviders = externalIssueProviders.ToList();
        _appNav = appNav;
        _loc = loc;
        _issueController = issueController;
        _stateService = stateService;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var issue = _appNav.State.SelectedIssue;
        if (issue is null)
        {
            return;
        }
        if(issue.TestProjectId is null)
        {
            return;
        }

        // find integration for Jira
        var integrations = await _projectManager.GetProjectIntegrationsAsync(principal, issue.TestProjectId.Value);
        var jira = integrations.FirstOrDefault(x => x.Name == TestBucket.Jira.ExtensionConstants.SystemName);
        if(jira is null)
        {
            return;
        }

        issue.ExternalSystemId = jira.Id;

        var linker = new LinkIssueWithIntegration(_projectManager, _externalIssueProviders, _stateService);
        await linker.Handle(principal, issue, CancellationToken.None);  

        // Save issue
        await _issueController.UpdateIssueAsync(issue);
    }
}
