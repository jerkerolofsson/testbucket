using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Contracts.Requirements;
using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Code.Mapping;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Mapping;
using TestBucket.Domain.Requirements.Search;

namespace TestBucket.Domain.Code.Mcp;

[McpServerToolType]
[DisplayName("code")]
public class CommitMcpTools : AuthenticatedTool
{
    private readonly ICommitManager _commitManager;
    private readonly TimeProvider _timeProvider;
    private readonly IRequirementManager _requirementManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    public CommitMcpTools(IArchitectureManager architecture,
        TimeProvider timeProvider,
        IApiKeyAuthenticator apiKeyAuthenticator, IRequirementManager requirementManager, IFieldDefinitionManager fieldDefinitionManager, ICommitManager commitManager) : base(apiKeyAuthenticator)
    {
        _timeProvider = timeProvider;
        _requirementManager = requirementManager;
        _fieldDefinitionManager = fieldDefinitionManager;
        _commitManager = commitManager;
    }

    public record class McpCommitDto
    {
        public long Id { get; set; }
        public required string Message { get; set; }
        public DateTimeOffset? CommitedTime { get; set; }
        public string? Sha { get; set; }
        public List<string>? ImpactedFeatures { get; set; }
        public List<string>? ImpactedComponents { get; set; }
    }

    [McpServerTool(Name = "get-recent-commits")]
    [Description("""
        Retreives a list of recent commits (code changes) and incudes impact SW components, features and fixed issues.
        """)]
    public async Task<IReadOnlyList<McpCommitDto>> GetRecentCommitsAsync(string searchText, int offset = 0, int count = 1)
    {
        if(count == 0)
        {
            count = 20;
        }

        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _commitManager.BrowseCommitsAsync(_principal, projectId.Value, offset, count);

                var items = result.Items.Select(x => new McpCommitDto 
                { 
                    Id = x.Id,
                    Message = x.Message ?? "",
                    CommitedTime = x.Commited,
                    Sha = x.Sha,
                    ImpactedFeatures = x.FeatureNames,
                    ImpactedComponents = x.ComponentNames
                }).ToList();

                return items;
            }
        }
        return [];
    }
}
