using TestBucket.Domain.Testing.Models;


public interface ITestCaseRepository
{
    Task AddTestCaseAsync(TestCase testCase);
    Task UpdateTestCaseAsync(TestCase testCase);
    Task<TestCase?> GetTestCaseByExternalIdAsync(string tenantId, long testSuiteId, string? externalId);
    Task<PagedResult<TestCase>> SearchTestCasesAsync(string tenantId, SearchTestQuery query);


    Task<TestSuiteFolder> AddTestSuiteFolderAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string name);
    Task<TestSuiteFolder?> GetTestSuiteFolderByNameAsync(string tenantId, long testsuiteId, long? parentId, string folderName);
    Task DeleteFolderByIdAsync(string tenantId, long folderId);
    Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId);
    Task UpdateTestSuiteFolderAsync(TestSuiteFolder folder);




    Task<TestSuite> AddTestSuiteAsync(string tenantId, long? teamId, long? projectId, string name);
    Task UpdateTestSuiteAsync(TestSuite suite);
    Task DeleteTestSuiteByIdAsync(string tenantId, long testSuiteId);
    Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, SearchQuery query);
    Task<TestSuite?> GetTestSuiteByNameAsync(string tenantId, long? teamId, long? projectId, string suiteName);

    Task AddTestRunAsync(TestRun testRun);
    Task AddTestCaseRunAsync(TestCaseRun testCaseRun);
}