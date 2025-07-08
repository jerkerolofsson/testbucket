using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Issues.Events;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;

namespace TestBucket.Domain.Issues.Integrations;
internal class CreateExternalIssuesWhenCreated : INotificationHandler<IssueCreated>
{
    private readonly IProjectManager _projectManager;
    private readonly List<IExternalIssueProvider> _externalIssueProviders;

    public CreateExternalIssuesWhenCreated(IProjectManager projectManager, IEnumerable<IExternalIssueProvider> externalIssueProviders)
    {
        _projectManager = projectManager;
        _externalIssueProviders = externalIssueProviders.ToList();
    }

    public async ValueTask Handle(IssueCreated notification, CancellationToken cancellationToken)
    {
        var issue = notification.Issue;
        var principal = notification.Principal;

        // Only create an external issue if the system id is set, but the external ID is not set.   
        // If the external id is set, that means we are importing the issue and it already exists in the external system
        if (issue.ExternalSystemId is not null && issue.ExternalId is null)
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
                        await provider.CreateIssueAsync(externalSystem.ToDto(), issueDto, cancellationToken);

                        // If the issue was successfully created, add the ID here..
                        if (!string.IsNullOrEmpty(issueDto.ExternalDisplayId))
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
