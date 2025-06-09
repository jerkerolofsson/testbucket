using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns.Search;
using TestBucket.Domain.Traceability.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Testing.TestCases;
public interface ITestCaseManager
{
    /// <summary>
    /// Adds an observer that will receive notifications when things are changing
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ITestCaseObserver observer);

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<TestCase> AddTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Deletes a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task DeleteTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);
    Task<TraceabilityNode> DiscoverTraceabilityAsync(ClaimsPrincipal principal, TestCase testCase, int depth);

    /// <summary>
    /// Duplicates a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<TestCase> DuplicateTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Returns a list of all items, starting with the root item until the test case including all folders in between
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestEntity>> ExpandUntilRootAsync(ClaimsPrincipal principal, TestCase testCase);
    Task<InsightsData<string, int>> GetInsightsTestCountPerFieldAsync(ClaimsPrincipal principal, SearchTestQuery query, TraitType traitType);

    /// <summary>
    /// Returns a test case by ID
    /// </summary>
    /// <param name="user"></param>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseByIdAsync(ClaimsPrincipal user, long testCaseId);

    /// <summary>
    /// Returns a test case by slug
    /// </summary>
    /// <param name="user"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseBySlugAsync(ClaimsPrincipal user, string slug);
    Task<Dictionary<string, Dictionary<string, long>>> GetTestCaseCoverageMatrixByFieldAsync(ClaimsPrincipal principal, SearchTestQuery query, long fieldDefinitionId1, long fieldDefinitionId2);
    Task<Dictionary<string, long>> GetTestCaseDistributionByFieldAsync(ClaimsPrincipal principal, SearchTestQuery query, long fieldDefinitionId);

    /// <summary>
    /// Returns test result summary for the provided runs
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRunsIds"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<Dictionary<long, TestExecutionResultSummary>> GetTestExecutionResultSummaryForRunsAsync(ClaimsPrincipal principal, IReadOnlyList<long> testRunsIds, SearchTestCaseRunQuery query);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ITestCaseObserver observer);

    /// <summary>
    /// Saves a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task SaveTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Searches REturns all test case ids
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<long> SearchTestCaseIdsAsync(ClaimsPrincipal principal, SearchTestQuery query, CancellationToken cancellationToken = default);
}