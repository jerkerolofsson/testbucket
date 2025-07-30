using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.TestRuns;
public interface ITestRunManager
{
    /// <summary>
    /// Adds an observer that will receive notifications when things are changing
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ITestRunObserver observer);

    /// <summary>
    /// Removes the observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ITestRunObserver observer);

    #region Test Runs

    Task<TestRun?> GetTestRunByIdAsync(ClaimsPrincipal principal, long id);
    Task<TestRun?> GetTestRunBySlugAsync(ClaimsPrincipal user, long? projectId, string slug);

    /// <summary>
    /// Adds a TestRun (a collection of test cases for execution)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task AddTestRunAsync(ClaimsPrincipal principal, TestRun testRun);

    /// <summary>
    /// Saves changes to a test run
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task SaveTestRunAsync(ClaimsPrincipal principal, TestRun testRun);

    /// <summary>
    /// Archives a run 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task ArchiveTestRunAsync(ClaimsPrincipal principal, TestRun testRun);

    /// <summary>
    /// Deletes a run and all references items such as test case runs
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task DeleteTestRunAsync(ClaimsPrincipal principal, TestRun testRun);

    /// <summary>
    /// Searches for test runs using a filter
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<TestRun>> SearchTestRunsAsync(ClaimsPrincipal principal, SearchTestRunQuery query);

    #endregion Test Runs

    #region Test Case Runs

    /// <summary>
    /// Creates a new TestCaseRun from a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task<TestCaseRun> AddTestCaseRunAsync(ClaimsPrincipal principal, TestRun testRun, TestCase testCase);
    Task DeleteTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun);


    /// <summary>
    /// Adds a TestCaseRun (a test case for execution)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task AddTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun);

    /// <summary>
    /// Saves changes made to a test case run
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task SaveTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun, bool informObservers = true);

    /// <summary>
    /// Searches for test case runs using a filter
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);

    /// <summary>
    /// Returns a summary report of results (passed, failed..) filtered by the query
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);
    Task<Dictionary<DateOnly, TestExecutionResultSummary>> GetTestExecutionResultSummaryByDayAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);
    Task<Dictionary<string, TestExecutionResultSummary>> GetTestExecutionResultSummaryByFieldAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query, long fieldDefinitionId);
    Task<TestCaseRun?> GetTestCaseRunByIdAsync(ClaimsPrincipal principal, long id);

    /// <summary>
    /// Duplicates the run including fields and tests
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="run"></param>
    /// <returns></returns>
    Task<TestRun> DuplicateTestRunAsync(ClaimsPrincipal principal, TestRun run);

    #endregion Test Case Runs


    Task<InsightsData<DateOnly, int>> GetInsightsTestResultsByDayAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);

    /// <summary>
    /// Returns counts per test result
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<InsightsData<TestResult, int>> GetInsightsTestResultsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);

    /// <summary>
    /// Returns counts per test result, where the result is only the latest run for each test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<InsightsData<TestResult, int>> GetInsightsLatestTestResultsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);
    Task<InsightsData<string, int>> GetInsightsTestResultsByFieldAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query, long fieldDefinitionId);
    Task<InsightsData<string, int>> GetInsightsTestCaseRunCountByAsigneeAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);

}
