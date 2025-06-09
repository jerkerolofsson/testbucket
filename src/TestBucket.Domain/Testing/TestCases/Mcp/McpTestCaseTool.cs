using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.ApiKeys;

namespace TestBucket.Domain.Testing.TestCases.Mcp;

[McpServerToolType]
public class McpTestCaseTool : AuthenticatedTool
{
    private readonly ITestCaseManager _testCaseManager;

    public McpTestCaseTool(ITestCaseManager testCaseManager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _testCaseManager = testCaseManager;
    }

    /// <summary>
    /// Returns the name of the user that is authenticated with the API key.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [McpServerTool, Description("Gets number of test cases per category (unit tests, integration tests, end-to-end tests, api tests..)")]
    public async Task<Dictionary<string,int>> GetNumberOfTestCasesByCategoryAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var result = await _testCaseManager.GetInsightsTestCountPerFieldAsync(_principal, new Models.SearchTestQuery
            {
                ProjectId = _principal.GetProjectId(),
            },
            Traits.Core.TraitType.TestCategory);

            var map = new Dictionary<string, int>();
            foreach(var series in result.Series)
            {
                foreach(var item in series.Data)
                {
                    if (item.Value > 0)
                    {
                        map[item.Label] = item.Value;
                    }
                }
            }
        }
        return [];
    }
}
