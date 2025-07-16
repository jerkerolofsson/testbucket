using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Requirements;
using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Requirements.Mapping;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Mcp;

[McpServerToolType]
public class RequirementMcpTools : AuthenticatedTool
{
    private readonly IRequirementManager _manager;

    public RequirementMcpTools(IRequirementManager manager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _manager = manager;
    }

    /// <summary>
    /// Assigns the issue to a user
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get-requirement-from-id"), 
        Description("""
        Returns information about a specific requirement using the requirementId to identify the requirement. 
        Only call this method with a valid requirement ID. 
        If you don't know the ID use the search-for-requirements tool.
        """)]
        
    public async Task<RequirementDto?> GetRequirementeFromIdAsync(string requirementId)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var issue = await FindRequirementAsync(requirementId, projectId.Value);
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
    /// Searches for requirements
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "search-for-requirements"), 
        Description("Searches for a requirements and returns the best match first")]
    public async Task<RequirementDto[]> SearchForRequirements(string text, int count = 1)
    {
        if(count == 0)
        {
            count = 10;
        }
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _manager.SemanticSearchRequirementsAsync(_principal, new SearchRequirementQuery
                {
                    CompareFolder = false,
                    ProjectId = projectId,
                    Text = text,
                    Offset = 0,
                    Count = count,
                });
                if (result is null || result.TotalCount == 0)
                {
                    return [];
                }

                return result.Items.Select(x => x.ToDto()).ToArray();
            }

        }
        return [];
    }

    private async Task<Requirement?> FindRequirementAsync(string requirementId, long projectId)
    {
        if (_principal is not null)
        {
            if (long.TryParse(requirementId, out var id))
            {
                var issue = await _manager.GetRequirementByIdAsync(_principal, id);
                if (issue is not null)
                {
                    return issue;
                }

            }

            var result = await _manager.SearchRequirementsAsync(_principal, new SearchRequirementQuery
            {
                ProjectId = projectId,
                CompareFolder = false,
                Text = requirementId,
                Offset = 0,
                Count = 1,
            });
            if (result is null || result.TotalCount == 0)
            {
                return null;
            }

            return result.Items[0];
        }
        return null;
    }

}
