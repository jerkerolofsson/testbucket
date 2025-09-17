using Mediator;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Issues.Events;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.States;

namespace TestBucket.Domain.Issues.Integrations;
internal class UpdateExternalIssuesWhenChanged : INotificationHandler<IssueChanged>
{
    private readonly IProjectManager _projectManager;
    private readonly List<IExternalIssueProvider> _externalIssueProviders;
    private readonly IStateService _stateService;

    public UpdateExternalIssuesWhenChanged(IProjectManager projectManager, IEnumerable<IExternalIssueProvider> externalIssueProviders, IStateService stateService)
    {
        _projectManager = projectManager;
        _externalIssueProviders = externalIssueProviders.ToList();
        _stateService = stateService;
    }

    public async ValueTask Handle(IssueChanged notification, CancellationToken cancellationToken)
    {
        var issue = notification.Issue;
        var principal = notification.Principal;
        if (issue.ExternalSystemId is not null && issue.ExternalId is not null)
        {
            // Find integration
            if (issue.TestProjectId is not null)
            {
                var externalSystems = await _projectManager.GetProjectIntegrationsAsync(principal, issue.TestProjectId.Value);
                var externalSystem = externalSystems.FirstOrDefault(x => x.Id == issue.ExternalSystemId);   
                if(externalSystem is not null)
                {
                    var provider = _externalIssueProviders.FirstOrDefault(x => x.SystemName == externalSystem.Name);
                    if (provider is not null)
                    {
                        // Update issue in external system
                        var issueDto = issue.ToDto();
                        var api = new ExtensionApi(_stateService, principal, issue.TestProjectId.Value);
                        await provider.UpdateIssueAsync(api, externalSystem.ToDto(), issueDto, cancellationToken);

                        // If the issue was successfully created, add the ID here..
                        if(!string.IsNullOrEmpty(issueDto.ExternalDisplayId))
                        {
                            issue.ExternalDisplayId = issueDto.ExternalDisplayId;
                            issue.ExternalId = issueDto.ExternalId;
                        }
                        else
                        {
                            // Failed, remove mapping
                            issue.ExternalSystemId = null;
                            issue.ExternalSystemName = null;
                        }
                    }
                }
            }
        }
    }
}
