using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;

namespace TestBucket.Domain.Issues.Mcp;

[McpServerToolType]
public class IssueMcpTools : AuthenticatedTool
{
    private readonly IIssueManager _manager;

    public IssueMcpTools(IIssueManager manager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _manager = manager;
    }

    /// <summary>
    /// Assigns the issue to a user
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get-issue-from-id"), Description("Returns information about a specific issue using the issueId to identify the issue")]
    public async Task<IssueDto?> GetIssueFromId(string issueId)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var issue = await FindIssueAsync(issueId);
                if (issue is null)
                {
                    return null;
                }

                return issue.ToDto();
            }

        }
        return null;
    }

    /// <summary>
    /// Searches for a specific issue
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "search-for-issue"), Description("Searches for an issue and returns the issue")]
    public async Task<IssueDto?> SearchForIssue(string text)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _manager.SearchLocalIssuesAsync(_principal, new SearchIssueQuery
                {
                    Text = text
                }, 0, 1);
                if (result is null || result.TotalCount == 0)
                {
                    return null;
                }

                return result.Items[0].ToDto();
            }

        }
        return null;
    }

    private async Task<LocalIssue?> FindIssueAsync(string issueId)
    {
        if (_principal is not null)
        {
            if (long.TryParse(issueId, out var id))
            {
                var issue = await _manager.GetIssueByIdAsync(_principal, id);
                if (issue is not null)
                {
                    return issue;
                }

            }

            var result = await _manager.SearchLocalIssuesAsync(_principal, new SearchIssueQuery
            {
                Text = issueId
            }, 0, 1);
            if (result is null || result.TotalCount == 0)
            {
                return null;
            }

            return result.Items[0];
        }
        return null;
    }

    /// <summary>
    /// Assigns the issue to a user
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "assign-issue"), Description("Assigns an issue to a user. If you don't know the issueId, search for the issue using the search-for-issue tool first.")]
    public async Task<string> AssignIssue(string issueId, string user)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var issue = await FindIssueAsync(issueId);
                if (issue is not null)
                {
                    issue.AssignedTo = user;
                    await _manager.UpdateLocalIssueAsync(_principal, issue);
                    return $"The issue with ID {issueId} was assigned to {user}";
                }
            }
        }
        return "You don't have permission to use the tool";
    }

    /// <summary>
    /// Returns the latest test results
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "list-open-issues"), Description("Returns a list of all open issues/bugs/tickets")]
    public async Task<PagedResult<IssueDto>> GetOpenIssues(int offset=0, int count=100)
    {
        if(count == 0)
        {
            count = 100;
        }

        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _manager.SearchLocalIssuesAsync(_principal, new SearchIssueQuery
                {
                    ProjectId = projectId,
                    MappedState = Contracts.Issues.States.MappedIssueState.Open
                }, 0, 100);

                var resultDto = new PagedResult<IssueDto>
                {
                    Items = result.Items.Select(i => i.ToDto()).ToArray(),
                    TotalCount = result.TotalCount
                };
                return resultDto;
            }

        }
        return new PagedResult<IssueDto> { Items = [], TotalCount = 0 };
    }
}
