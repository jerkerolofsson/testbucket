using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Testing.Mapping;
using TestBucket.Domain.Testing.TestSuites.Search;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.TestSuites.Mcp;

[McpServerToolType]
public class TestSuitesMcpTools : AuthenticatedTool
{
    private readonly ITestSuiteManager _testSuiteManager;

    public TestSuitesMcpTools(ITestSuiteManager testSuiteManager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _testSuiteManager = testSuiteManager;
    }

    /// <summary>
    /// Searches for test suites
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "search-test-suites"), Description("Searches for test suites")]
    public async Task<IReadOnlyList<TestSuiteDto>> SearchTestSuitesAsync(string searchText, int offset, int count)
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var result = await _testSuiteManager.SearchTestSuitesAsync(_principal, new SearchTestSuiteQuery
            {
                ProjectId = _principal.GetProjectId(),
                Text = searchText,
                Offset = offset,
                Count = count
            });

            return new List<TestSuiteDto>(result.Items.Select(x=>TestSuiteMapper.ToDto(x)));
        }
        return [];
    }
}
