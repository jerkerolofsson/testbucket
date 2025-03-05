
using TestBucket.Components.Tenants;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Components.Tests.Services;

internal class TestSuiteService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;

    public TestSuiteService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
    }

    /// <summary>
    /// Saves a folder
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public async Task SaveTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        var tenantId = await GetTenantIdAsync();
        if (folder.TenantId != tenantId)
        {
            throw new InvalidOperationException("TenantId mismatch");
        }

        await _testCaseRepo.UpdateTestSuiteFolderAsync(folder);
    }

    /// <summary>
    /// Adds a folder
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="parentFolderId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(long? projectId, long testSuiteId, long? parentFolderId, string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.AddTestSuiteFolderAsync(tenantId, projectId, testSuiteId, parentFolderId, name);
    }

    /// <summary>
    /// Gets folders
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="parentFolderId"></param>
    /// <returns></returns>
    public async Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(long? projectId, long testSuiteId, long? parentFolderId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.GetTestSuiteFoldersAsync(tenantId, projectId, testSuiteId, parentFolderId);
    }


    /// <summary>
    /// Deletes a folder
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public async Task DeleteFolderByIdAsync(long folderId)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteFolderByIdAsync(tenantId, folderId);
    }

    /// <summary>
    /// Deletes a test suite
    /// </summary>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    public async Task DeleteTestSuiteByIdAsync(long testSuiteId)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteTestSuiteByIdAsync(tenantId, testSuiteId);
    }

    /// <summary>
    /// Adds a test suite
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<TestSuite> AddTestSuiteAsync(long? teamId, long? projectId, string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.AddTestSuiteAsync(tenantId, teamId, projectId, name);
    }

    /// <summary>
    /// Saves a test suite
    /// </summary>
    /// <param name="suite"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SaveTestSuiteAsync(TestSuite suite)
    {
        var tenantId = await GetTenantIdAsync();
        if (suite.TenantId != tenantId)
        {
            throw new InvalidOperationException("TenantId mismatch");
        }
        await _testCaseRepo.UpdateTestSuiteAsync(suite);
    }

    public async Task<PagedResult<TestSuite>> GetTestSuitesAsync(long? teamId, long? projectId, int offset=0, int count = 100)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.SearchTestSuitesAsync(tenantId, new SearchQuery
        {
            TeamId = teamId,
            ProjectId = projectId,
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
