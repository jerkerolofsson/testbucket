using System.Xml.Linq;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.CodeAnalysis;

using TestBucket.Components.Shared;
using TestBucket.Contracts;
using TestBucket.Data.Testing;
using TestBucket.Data.Testing.Models;

namespace TestBucket.Components.Tests.Services;

internal class TestSuiteService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;

    public TestSuiteService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
    }

    public async Task AddTestCaseAsync(TestCase testCase)
    {
        testCase.TenantId = await GetTenantIdAsync();
        await _testCaseRepo.AddTestCaseAsync(testCase);
    }

    public async Task DeleteFolderByIdAsync(long testSuiteId)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteFolderByIdAsync(tenantId, testSuiteId);
    }

    public async Task<TestSuite> AddTestSuiteAsync(long? projectId, string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.AddTestSuiteAsync(tenantId, projectId, name);
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
