using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.TestRuns.Search;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Testing.TestRuns.Mcp;

[McpServerToolType]
[DisplayName("testing")]
public class TestResultsMcpTools : AuthenticatedTool
{
    private readonly ITestRunManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public TestResultsMcpTools(ITestRunManager manager, IApiKeyAuthenticator apiKeyAuthenticator, IFieldDefinitionManager fieldDefinitionManager) : base(apiKeyAuthenticator)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    /// <summary>
    /// Returns the latest test results
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get-latest-test-results-for-milestone"), Description("Returns the latest test results for a specific milestone. This only returns test-results, no information about the milestones.")]
    public async Task<Dictionary<string, int>> GetLatestTestResultsForMilestoneAsync(string milestone)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(_principal, projectId);
            var fieldFilter = new FieldFilter 
            { 
                FilterDefinitionId = fieldDefinitions.FirstOrDefault(f => f.TraitType == TraitType.Milestone)!.Id, 
                StringValue = milestone,
                Name = "Milestone"
            };
            var result = await _manager.GetInsightsLatestTestResultsAsync(_principal, new SearchTestCaseRunQuery
            {
                Fields = [fieldFilter],
                ProjectId = _principal.GetProjectId(),
            });
            return ToDict(result);
        }
        return [];
    }

    /// <summary>
    /// Returns the latest test results
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get-latest-test-results"), Description("Returns the latest test results (passed count, failed count..)")]
    public async Task<Dictionary<string,int>> GetLatestTestResultsAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var result = await _manager.GetInsightsLatestTestResultsAsync(_principal, new SearchTestCaseRunQuery
            {
                ProjectId = _principal.GetProjectId(),
            });
            return ToDict(result);
        }
        return [];
    }

    private static Dictionary<string, int> ToDict(Domain.Insights.Model.InsightsData<TestResult, int> result)
    {
        var map = new Dictionary<string, int>();
        foreach (var series in result.Series)
        {
            foreach (var item in series.Data)
            {
                if (item.Value > 0)
                {
                    map[item.Label.ToString()] = item.Value;
                }
            }
        }
        return map;
    }
}
