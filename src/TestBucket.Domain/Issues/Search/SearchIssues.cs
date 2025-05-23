﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;

using static TestBucket.Domain.Issues.Search.SearchIssueRequest;

namespace TestBucket.Domain.Issues.Search;

public record class SearchIssueRequest(ClaimsPrincipal Principal, SearchIssueQuery Query) : IRequest<SearchIssueResponse>;

public class SearchIssueQuery : SearchQuery
{
    public string? State { get; set; }
    public MappedIssueState? MappedState { get; set; }
    public string? Type { get; set; }
    public string? ExternalSystemName { get; set; }
    public long? ExternalSystemId { get; set; }
    public string? AssignedToUser { get; set; }
}

public record class SearchIssueResponse(IReadOnlyList<IssueDto> Issues);

public class SearchIssuesHandler : IRequestHandler<SearchIssueRequest, SearchIssueResponse>
{
    private readonly IProjectManager _projectManager;
    private readonly IIssueManager _issueManager;
    private readonly IReadOnlyList<IExternalIssueProvider> _issueExtensions;

    public SearchIssuesHandler(
        IProjectManager projectManager,
        IEnumerable<IExternalIssueProvider> issueExtensions,
        IIssueManager issuemanager,
        IIssueManager issueManager)
    {
        _projectManager = projectManager;
        _issueExtensions = issueExtensions.ToList();
        _issueManager = issueManager;
    }

    public async ValueTask<SearchIssueResponse> Handle(SearchIssueRequest request, CancellationToken cancellationToken)
    {
        List<IssueDto> issues = [];
        int offset = 0;
        int count = 10;

        // Search locally
        var result = await _issueManager.SearchLocalIssuesAsync(request.Principal, request.Query, offset, count);
        foreach(var linkedIssue in result.Items)
        {
            issues.Add(linkedIssue.ToDto());
        }
        return new SearchIssueResponse(issues);
    }
}
