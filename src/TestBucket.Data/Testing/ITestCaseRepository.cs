
namespace TestBucket.Data.Testing;

public interface ITestCaseRepository
{
    Task AddTestCaseAsync(TestCase testCase);
    Task<TestSuite> AddTestSuiteAsync(string tenantId, long? projectId, string name);
    Task<TestSuiteFolder> AddTestSuiteFolderAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string name);
    Task DeleteFolderByIdAsync(string tenantId, long folderId);
    Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId);
    Task<PagedResult<TestCase>> SearchTestCasesAsync(string tenantId, SearchTestQuery query);
    Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, long? projectId, SearchQuery query);
}