﻿using Mediator;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Issues.Events;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;

namespace TestBucket.Domain.Issues.Integrations;
internal class UpdateExternalIssuesWhenChanged : INotificationHandler<IssueChanged>
{
    private readonly IProjectManager _projectManager;
    private readonly List<IExternalIssueProvider> _externalIssueProviders;

    public UpdateExternalIssuesWhenChanged(IProjectManager projectManager, IEnumerable<IExternalIssueProvider> externalIssueProviders)
    {
        _projectManager = projectManager;
        _externalIssueProviders = externalIssueProviders.ToList();
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
                        await provider.UpdateIssueAsync(externalSystem.ToDto(), issueDto, cancellationToken);

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
