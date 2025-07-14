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
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Search;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Code.Mcp;

[McpServerToolType]
public class CodeMcpTools : AuthenticatedTool
{
    private readonly IArchitectureManager _architectureManager;
    private readonly TimeProvider _timeProvider;
    private readonly IRequirementManager _requirementManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public CodeMcpTools(IArchitectureManager architecture,
        TimeProvider timeProvider,
        IApiKeyAuthenticator apiKeyAuthenticator, IRequirementManager requirementManager, IFieldDefinitionManager fieldDefinitionManager) : base(apiKeyAuthenticator)
    {
        _architectureManager = architecture;
        _timeProvider = timeProvider;
        _requirementManager = requirementManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public record class FeatureMcpDto
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public IReadOnlyList<RequirementDto> RelatedRequirements { get; set; } = [];
    }

    [McpServerTool(Name = "search-features"), Description("""
        Searches for a feature, returning description and information about the feature and related requirements.
        """)]
    public async Task<IReadOnlyList<FeatureMcpDto>> SearchFeaturesAsync(string searchText, int offset = 0, int count = 1)
    {
        if(count == 0)
        {
            count = 1;
        }

        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _architectureManager.SearchFeaturesAsync(_principal, projectId.Value, searchText, offset, count);

                if(result.Items.Length == 0)
                {
                    // Sometimes the llm adds "feature" in the searchText
                    if(searchText.Contains("features", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchText = searchText.Replace("features", "", StringComparison.InvariantCultureIgnoreCase);
                    }
                    result = await _architectureManager.SearchFeaturesAsync(_principal, projectId.Value, searchText, offset, count);
                }

                var items = result.Items.Select(x => new FeatureMcpDto 
                { 
                    Id = x.Id,
                    Description = x.Description ?? "",
                    Name = x.Name, 
                }).ToList();

                // Find requirements related to this feature

                var fields = await _fieldDefinitionManager.GetDefinitionsAsync(_principal, projectId, Contracts.Fields.FieldTarget.Requirement);

                foreach (var item in items)
                {
                    string queryText = $"feature:\"{item.Name}\"";
                    var query = SearchRequirementQueryParser.Parse(queryText, fields, _timeProvider);
                    query.Count = 100;
                    query.Offset = 0;
                    query.ProjectId = projectId.Value;
                    query.CompareFolder = false;
                    var requirements = await _requirementManager.SearchRequirementsAsync(_principal, query);

                    item.RelatedRequirements = requirements.Items.Select(x => x.ToDto()).ToList();
                }

                return items;
            }
        }
        return [];
    }
}
