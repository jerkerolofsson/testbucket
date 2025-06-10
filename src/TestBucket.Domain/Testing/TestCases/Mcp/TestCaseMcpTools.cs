using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases.Mcp;

[McpServerToolType]
public class TestCaseMcpTools : AuthenticatedTool
{
    private readonly ITestCaseManager _testCaseManager;

    public TestCaseMcpTools(ITestCaseManager testCaseManager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _testCaseManager = testCaseManager;
    }

    [McpServerTool(Name = "add-test-case"), Description("Adds a test case with a name and description to an existing test suite")]
    public async Task AddTestCase(long testSuiteId, string testCaseName, string testCaseDescription)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var testCase = new TestCase
            {
                TestProjectId = _principal.GetProjectId(),
                TestSuiteId = testSuiteId,
                Name = testCaseName,
                Description = testCaseDescription
            };
            await _testCaseManager.AddTestCaseAsync(_principal, testCase);
        }
    }

    /// <summary>
    /// Returns the name of the user that is authenticated with the API key.
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get-number-of-testcases-by-category"), Description("Gets number of test cases per category (unit tests, integration tests, end-to-end tests, api tests..)")]
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
            return map;
        }
        return [];
    }
}
