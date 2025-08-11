using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestSuites.Search;


public interface ITestCaseRepository : ITestRunRepository
{
    #region Test Case

    /// <summary>
    /// Returns a test case by slug
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseBySlugAsync(string tenantId, long? projectId, string slug);

    /// <summary>
    /// Returns a test case by name
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId">If null all projects are checked</param>
    /// <param name="testSuiteId">If null all test suites are checked</param>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseByNameAsync(string tenantId, long? projectId, long? testSuiteId, string name);

    /// <summary>
    /// Returns a test case by ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseByIdAsync(string tenantId, long testCaseId);

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task AddTestCaseAsync(TestCase testCase);

    /// <summary>
    /// Updates a test case / saves changes
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task UpdateTestCaseAsync(TestCase testCase);

    /// <summary>
    /// Searches for a test by an external ID
    /// This is used when importing to avoid creating duplicates
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="externalId"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseByExternalIdAsync(string tenantId, string? externalId);

    /// <summary>
    /// Returns a test case from implementation details
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <param name="assemblyName"></param>
    /// <param name="module"></param>
    /// <param name="className"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseByAutomationImplementationAttributesAsync(string tenantId, string? assemblyName, string? module, string? className, string? method);

    /// <summary>
    /// Searches for test cases
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    Task<PagedResult<TestCase>> SearchTestCasesAsync(int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters, Func<TestCase, object> orderBy, bool descending);

    /// <summary>
    /// Searches for test cases
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    Task<long[]> SearchTestCaseIdsAsync(IEnumerable<FilterSpecification<TestCase>> filters);

    /// <summary>
    /// Deletes a test case
    /// </summary>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    Task DeleteTestCaseByIdAsync(long testCaseId);

    #endregion

    #region Test Suite Folders

    /// <summary>
    /// Returns a folder by ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="folderId"></param>
    /// <returns></returns>
    Task<TestSuiteFolder?> GetTestSuiteFolderByIdAsync(string tenantId, long folderId);

    /// <summary>
    /// Gets a test suite folder by name
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="parentFolderId">Parent location</param>
    /// <param name="folder">Name of folder</param>
    /// <returns></returns>
    Task<TestSuiteFolder?> GetTestSuiteFolderByNameAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string folder);

    /// <summary>
    /// Adds a folder
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="parentFolderId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<TestSuiteFolder> AddTestSuiteFolderAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string name);

    /// <summary>
    /// Gets a test suite folder by name
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testsuiteId"></param>
    /// <param name="parentId"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    Task<TestSuiteFolder?> GetTestSuiteFolderByNameAsync(string tenantId, long testsuiteId, long? parentId, string folderName);

    /// <summary>
    /// Deletes a folder
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="folderId"></param>
    /// <returns></returns>
    Task DeleteFolderByIdAsync(string tenantId, long folderId);

    /// <summary>
    /// Returns test suite folders
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="parentFolderId"></param>
    /// <returns></returns>
    Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId);

    /// <summary>
    /// Updates a folder / saves changes
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    Task UpdateTestSuiteFolderAsync(TestSuiteFolder folder);

    #endregion Test Suite Folders

    #region Test Suites
    /// <summary>
    /// Returns a test suite by ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TestSuite?> GetTestSuiteByIdAsync(string tenantId, long id);

    /// <summary>
    /// Returns a test suite by slug
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<TestSuite?> GetTestSuiteBySlugAsync(string tenantId, long? projectId, string slug);
    /// <summary>
    /// Adds a test suite
    /// </summary>
    /// <returns></returns>
    Task<TestSuite> AddTestSuiteAsync(TestSuite testSuite);

    /// <summary>
    /// Updates a test suite / saves changes
    /// </summary>
    /// <param name="suite"></param>
    /// <returns></returns>
    Task UpdateTestSuiteAsync(TestSuite suite);

    /// <summary>
    /// Deletes a test sutie
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    Task DeleteTestSuiteByIdAsync(string tenantId, long testSuiteId);

    /// <summary>
    /// Searches for test suites
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, SearchTestSuiteQuery query);

    /// <summary>
    /// Returns a test suite by name or null if not found
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="suiteName"></param>
    /// <returns></returns>
    Task<TestSuite?> GetTestSuiteByNameAsync(string tenantId, long? teamId, long? projectId, string suiteName);
    #endregion Test Suites

    #region Test Runs

    /// <summary>
    /// Returns run years
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<int[]> GetTestRunYearsAsync(string tenantId, long? teamId, long? projectId);

    #endregion Test Runs

    #region Test Case Runs

    /// <summary>
    /// Searches for test case runs
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="searchTestQuery"></param>
    /// <returns></returns>
    Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(IReadOnlyList<FilterSpecification<TestCaseRun>> filters, int offset, int count);

    Task<TestCaseRun?> GetTestCaseRunByIdAsync(string tenantId, long testCaseId);

    /// <summary>
    /// Deletes a test case run
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task DeleteTestCaseRunAsync(TestCaseRun testCaseRun);

    /// <summary>
    /// Adds a test case run
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task AddTestCaseRunAsync(TestCaseRun testCaseRun);

    /// <summary>
    /// Updates a test case run / saves changes
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task UpdateTestCaseRunAsync(TestCaseRun testCaseRun);

    #endregion

    #region Queries

    /// <summary>
    /// Returns a summary containing passed tests, total tests based on the filter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters);
    Task<Dictionary<string, TestExecutionResultSummary>> GetTestExecutionResultSummaryByFieldAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters, long fieldDefinitionId);
    Task<Dictionary<DateOnly, TestExecutionResultSummary>> GetTestExecutionResultSummaryByDayAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters);
    Task<TestRun?> GetTestRunBySlugAsync(string tenantId, long? projectId, string slug);
    Task<Dictionary<string, long>> GetTestCaseDistributionByFieldAsync(List<FilterSpecification<TestCase>> filters, long fieldDefinitionId);
    Task<Dictionary<string, Dictionary<string, long>>> GetTestCaseCoverageMatrixByFieldAsync(List<FilterSpecification<TestCase>> filters, long fieldDefinitionId1, long fieldDefinitionId2);

    /// <summary>
    /// Returns results for each provided run
    /// </summary>
    /// <param name="testRunsIds"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    Task<Dictionary<long, TestExecutionResultSummary>> GetTestExecutionResultSummaryForRunsAsync(IReadOnlyList<long> testRunsIds, IEnumerable<FilterSpecification<TestCaseRun>> filters);
    Task<InsightsData<DateOnly, int>> GetInsightsTestResultsByDayAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters);
    Task<InsightsData<TestResult, int>> GetInsightsTestResultsAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters);
    Task<InsightsData<TestResult, int>> GetInsightsLatestTestResultsAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters);
    Task<InsightsData<string, int>> GetInsightsTestCountPerFieldAsync(IEnumerable<FilterSpecification<TestCase>> filters, long fieldDefinitionId);
    Task<InsightsData<string, int>> GetInsightsTestResultsByFieldAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters, long fieldDefinitionId);
    Task<InsightsData<string, int>> GetInsightsTestCaseRunCountByAssigneeAsync(List<FilterSpecification<TestCaseRun>> filters);

    #endregion

    Task AddTestRepositoryFolderAsync(TestRepositoryFolder folder);
    Task AddTestLabFolderAsync(TestLabFolder folder);
    Task<IReadOnlyList<TestRepositoryFolder>> GetRootTestRepositoryFoldersAsync(long projectId);
    Task<IReadOnlyList<TestLabFolder>> GetRootTestLabFoldersAsync(long projectId);
    Task<IReadOnlyList<TestLabFolder>> GetChildTestLabFoldersAsync(long projectId, long parentId);
    Task<IReadOnlyList<TestRepositoryFolder>> GetChildTestRepositoryFoldersAsync(long projectId, long parentId);
    Task UpdateTestRepositoryFolderAsync(TestRepositoryFolder folder);
    Task UpdateTestLabFolderAsync(TestLabFolder folder);
    Task DeleteTestRepositoryFolderAsync(TestRepositoryFolder folder);
    Task DeleteTestLabFolderAsync(TestLabFolder folder);
    Task<TestRepositoryFolder?> GetTestRepositoryFolderByIdAsync(long folderId);
    Task<TestLabFolder?> GetTestLabFolderByIdAsync(long folderId);
    Task<PagedResult<TestCase>> SemanticSearchTestCasesAsync(ReadOnlyMemory<float> embeddingVector, int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters);
}