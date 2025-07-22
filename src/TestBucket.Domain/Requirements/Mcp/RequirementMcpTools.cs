using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Contracts.Requirements;
using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements.Mapping;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Search;

namespace TestBucket.Domain.Requirements.Mcp;

[McpServerToolType]
[DisplayName("specifications")]
public class RequirementMcpTools : AuthenticatedTool
{
    private readonly IRequirementManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IProjectManager _projectManager;

    public RequirementMcpTools(IRequirementManager manager, IApiKeyAuthenticator apiKeyAuthenticator, IFieldDefinitionManager fieldDefinitionManager, IProjectManager projectManager) : base(apiKeyAuthenticator)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
        _projectManager = projectManager;
    }

    /// <summary>
    /// Assigns the issue to a user
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get_requirement_from_id"), 
        Description("""
        Returns information about a specific requirement using the requirementId to identify the requirement. 
        Only call this method with a valid requirement ID. 
        If you don't know the ID use the search_requirements tool.
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
    [McpServerTool(Name = "search_requirements"), 
        Description("""
        Searches for a requirements.
        - If you want to search for a feature, use the query: "feature:"FEATURE_NAME" where FEATURE_NAME is the name of the feature you want to search for. FEATURE_NAME should be in quotes if it contains a space.
        - If you want to search for a component, use the query: "component:"COMPONENT_NAME" where COMPONENT_NAME is the name of the component you want to search for. COMPONENT_NAME should be in quotes if it contains a space
        """)]
        
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
                var fields = await _fieldDefinitionManager.GetDefinitionsAsync(_principal, projectId.Value, Contracts.Fields.FieldTarget.Requirement);
                var searchQuery = SearchRequirementQueryParser.Parse(text, fields);
                searchQuery.Count = count;
                searchQuery.ProjectId = projectId.Value;
                searchQuery.Offset = 0;

                var result = await _manager.SemanticSearchRequirementsAsync(_principal, searchQuery);
                if (result is null || result.TotalCount == 0)
                {
                    return [];
                }

                return result.Items.Select(x => x.ToDto()).ToArray();
            }

        }
        return [];
    }


    [McpServerTool(Name = "add_requirement"),
        Description("Adds a single requirement with a title and description")]
    public async Task AddTestCase(
        [Description("A descriptive name for the test case summarizing what it does")] string title,
        [Description("A brief description of the test case")] string description)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is null)
            {
                throw new ArgumentException("The user was authenticated but the project was defined in the claims");
            }

            // If there is no test suite, create a new one
            long specificaitonId = await CreateDefaultSpecificationIfNotExistsAsync(0, projectId);

            var requirement = new Requirement
            {
                RequirementSpecificationId = specificaitonId,
                TestProjectId = projectId,
                Name = title,
                Description = description
            };
            await _manager.AddRequirementAsync(_principal, requirement);
        }
    }


    private async Task<long> CreateDefaultSpecificationIfNotExistsAsync(long requirementSpecificationId, long? projectId)
    {
        ArgumentNullException.ThrowIfNull(_principal);

        var requirementSpecification = await _manager.GetRequirementSpecificationByIdAsync(_principal, requirementSpecificationId);
        if (requirementSpecification is null)
        {
            var defaultSuiteName = "Generated Requirements";
            var suites = await _manager.SearchRequirementSpecificationsAsync(_principal, new SearchQuery
            {
                ProjectId = projectId,
                Text = defaultSuiteName,
                Offset = 0,
                Count = 1,
            });
            if (suites.Items.Length == 0 && projectId is not null)
            {
                var project = await _projectManager.GetTestProjectByIdAsync(_principal, projectId.Value);
                var specification = new RequirementSpecification
                {
                    TeamId = project?.TeamId,
                    TestProjectId = projectId,
                    Name = defaultSuiteName,
                    Description = "Automatically generated requirements"
                };
                await _manager.AddRequirementSpecificationAsync(_principal, specification);
                requirementSpecificationId = specification.Id;
            }
            else
            {
                requirementSpecification = suites.Items[0];
                requirementSpecificationId = requirementSpecification.Id;
            }
        }

        return requirementSpecificationId;
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
