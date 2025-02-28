
namespace TestBucket.Components.Tests.Services;

internal class TestSuiteService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;

    public TestSuiteService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
    }


    public async Task DeleteFolderByIdAsync(long folderId)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteFolderByIdAsync(tenantId, folderId);
    }
    public async Task DeleteTestSuiteByIdAsync(long testSuiteId)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteTestSuiteByIdAsync(tenantId, testSuiteId);
    }

    public async Task<TestSuite> AddTestSuiteAsync(long? projectId, string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.AddTestSuiteAsync(tenantId, projectId, name);
    }

    public async Task SaveTestSuiteAsync(TestSuite suite)
    {
        var tenantId = await GetTenantIdAsync();
        if (suite.TenantId != tenantId)
        {
            throw new InvalidOperationException("TenantId mismatch");
        }
        await _testCaseRepo.UpdateTestSuiteAsync(suite);
    }


    public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(long? projectId, long testSuiteId, long? parentFolderId, string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.AddTestSuiteFolderAsync(tenantId, projectId, testSuiteId, parentFolderId, name);
    }


    public async Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(long? projectId, long testSuiteId, long? parentFolderId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.GetTestSuiteFoldersAsync(tenantId, projectId, testSuiteId, parentFolderId);
    }


    public async Task<PagedResult<TestSuite>> GetTestSuitesAsync(long? projectId, int offset=0, int count = 100)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.SearchTestSuitesAsync(tenantId, projectId, new SearchQuery
        {
            Offset = offset,
            Count = count
        });
    }

    internal async Task<PagedResult<TestCase>> SearchTestCasesAsync(SearchTestQuery searchTestQuery)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.SearchTestCasesAsync(tenantId, searchTestQuery);
    }
}
