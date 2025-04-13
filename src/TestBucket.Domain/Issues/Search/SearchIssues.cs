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

public record class SearchIssueRequest(ClaimsPrincipal Principal, long ProjectId, string Text, int Offset, int Count) : IRequest<SearchIssueResponse>;
public record class SearchIssueResponse(IReadOnlyList<IssueDto> Issues);

public class SearchIssuesHandler : IRequestHandler<SearchIssueRequest, SearchIssueResponse>
{
    private readonly IProjectManager _projectManager;
    //private readonly IIssueRepository _issueRepository;
    private readonly IReadOnlyList<IExternalIssueProvider> _issueExtensions;

    public SearchIssuesHandler(
        IProjectManager projectManager, 
        IEnumerable<IExternalIssueProvider> issueExtensions)
    {
        _projectManager = projectManager;
        _issueExtensions = issueExtensions.ToList();
        //_issueRepository = issueRepository;
    }

    public async ValueTask<SearchIssueResponse> Handle(SearchIssueRequest request, CancellationToken cancellationToken)
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
                var extensionIssues = await extension.SearchAsync(config.ToDto(), request.Text, request.Offset, request.Count, cancellationToken);
                foreach(var extensionIssue in extensionIssues)
                {
                    issues.Add(extensionIssue);
                }
            }
        }
        return new SearchIssueResponse(issues);
    }
}
