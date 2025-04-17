using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;

namespace TestBucket.Domain.Issues.Search;

public record class RefreshIssueRequest(ClaimsPrincipal Principal, long ProjectId, string ExternalIssueID) : IRequest<RefreshIssueResponse>;
public record class RefreshIssueResponse(IssueDto? Issue);

public class RefreshIssueHandler : IRequestHandler<RefreshIssueRequest, RefreshIssueResponse>
{
    private readonly IProjectManager _projectManager;
    //private readonly IIssueRepository _issueRepository;
    private readonly IReadOnlyList<IExternalIssueProvider> _issueExtensions;

    public RefreshIssueHandler(
        IProjectManager projectManager, 
        IEnumerable<IExternalIssueProvider> issueExtensions)
    {
        _projectManager = projectManager;
        _issueExtensions = issueExtensions.ToList();
        //_issueRepository = issueRepository;
    }

    public async ValueTask<RefreshIssueResponse> Handle(RefreshIssueRequest request, CancellationToken cancellationToken)
    {
        List<IssueDto> issues = [];

        var integrations = await _projectManager.GetProjectIntegrationsAsync(request.Principal, request.ProjectId);
        var issueIntegrations = integrations.Where(x => (x.SupportedCapabilities & ExternalSystemCapability.GetIssues) == ExternalSystemCapability.GetIssues);
        foreach(var config in issueIntegrations)
        {
            if(config.Provider is null)
            {
                continue;
            }

            foreach(var extension in _issueExtensions.Where(x=>x.SystemName ==  config.Provider))
            {
                return new RefreshIssueResponse(await extension.GetIssueAsync(config.ToDto(), request.ExternalIssueID, cancellationToken));
            }
        }
        return new RefreshIssueResponse(null);
    }
}
