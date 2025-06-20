using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites;

namespace TestBucket.Domain.Testing.TestCases.Mcp;

[McpServerToolType]
public class TestCaseMcpTools : AuthenticatedTool
{
    private readonly ITestCaseManager _testCaseManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IProjectManager _projectManager;

    public TestCaseMcpTools(ITestCaseManager testCaseManager, IApiKeyAuthenticator apiKeyAuthenticator, ITestSuiteManager testSuiteManager, IProjectManager projectManager) : base(apiKeyAuthenticator)
    {
        _testCaseManager = testCaseManager;
        _testSuiteManager = testSuiteManager;
        _projectManager = projectManager;
    }

    [McpServerTool(Name = "add-test-case"), Description("Adds a test case with a name and description to an existing test suite")]
    public async Task AddTestCase(long testSuiteId, string testCaseName, string testCaseDescription)
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
            testSuiteId = await CreateDefaultTestSuiteIfNotExistsAsync(testSuiteId, projectId);

            var testCase = new TestCase
            {
                TestProjectId = projectId,
                TestSuiteId = testSuiteId,
                Name = testCaseName,
                Description = testCaseDescription
            };
            await _testCaseManager.AddTestCaseAsync(_principal, testCase);
        }
    }

    private async Task<long> CreateDefaultTestSuiteIfNotExistsAsync(long testSuiteId, long? projectId)
    {
        ArgumentNullException.ThrowIfNull(_principal);

        var testSuite = await _testSuiteManager.GetTestSuiteByIdAsync(_principal, testSuiteId);
        if (testSuite is null)
        {
            var defaultSuiteName = "Generated Test Cases";
            var suites = await _testSuiteManager.SearchTestSuitesAsync(_principal, new TestSuites.Search.SearchTestSuiteQuery
            {
                ProjectId = projectId,
                Text = defaultSuiteName,
                Offset = 0,
                Count = 1,
            });
            if (suites.Items.Length == 0 && projectId is not null)
            {
                var project = await _projectManager.GetTestProjectByIdAsync(_principal, projectId.Value);
                var newTestSuite = new TestSuite
                {
                    TeamId = project?.TeamId,
                    TestProjectId = projectId,
                    Name = defaultSuiteName,
                    Description = "Automatically generated test cases"
                };
                testSuite = await _testSuiteManager.AddTestSuiteAsync(_principal, newTestSuite);
                testSuiteId = testSuite.Id;
            }
            else
            {
                testSuite = suites.Items[0];
                testSuiteId = testSuite.Id;
            }
        }

        return testSuiteId;
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
