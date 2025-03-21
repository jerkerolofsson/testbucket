using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;


public interface ITestCaseRepository
{
    #region Test Case
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
    Task<PagedResult<TestCase>> SearchTestCasesAsync(int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters);

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
    Task<TestSuite?> GetTestSuiteByIdAsync(string tenantId, long id);

    /// <summary>
    /// Adds a test suite
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<TestSuite> AddTestSuiteAsync(string tenantId, long? teamId, long? projectId, string name);

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
    Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, SearchQuery query);

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

    Task<TestCaseRun?> GetTestCaseRunByIdAsync(string tenantId, long testCaseId);
    Task<TestRun?> GetTestRunByIdAsync(string tenantId, long id);

    /// <summary>
    /// Searches for test case runs
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="searchTestQuery"></param>
    /// <returns></returns>
    Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(string tenantId, SearchTestQuery searchTestQuery);

    /// <summary>
    /// Returns run years
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<int[]> GetTestRunYearsAsync(string tenantId, long? teamId, long? projectId);

    /// <summary>
    /// Adds a run
    /// </summary>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task AddTestRunAsync(TestRun testRun);

    /// <summary>
    /// Adds a test case run
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task AddTestCaseRunAsync(TestCaseRun testCaseRun);

    /// <summary>
    /// Searches for test runs
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<TestRun>> SearchTestRunsAsync(string tenantId, SearchQuery query);

    /// <summary>
    /// Deletes a test run
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteTestRunByIdAsync(long id);

    /// <summary>
    /// Updates a test case run / saves changes
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    Task UpdateTestCaseRunAsync(TestCaseRun testCaseRun);

    #endregion
}