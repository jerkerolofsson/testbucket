using TestBucket.Components.Tests.TestSuites.Dialogs;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Components.Tests.TestSuites.Services;

internal class TestSuiteService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IDialogService _dialogService;

    public TestSuiteService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider,
        IFieldDefinitionManager fieldDefinitionManager,
        ITestSuiteManager testSuiteManager,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
        _fieldDefinitionManager = fieldDefinitionManager;
        _testSuiteManager = testSuiteManager;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Saves a folder
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public async Task SaveTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testSuiteManager.SaveTestSuiteFolderAsync(principal, folder);
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
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.AddTestSuiteFolderAsync(principal, projectId, testSuiteId, parentFolderId, name);
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
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.GetTestSuiteFoldersAsync(principal, projectId, testSuiteId, parentFolderId);
    }


    public async Task<TestSuiteFolder?> PickFolderAsync(TestProject project)
    {
        var parameters = new DialogParameters<PickTestFolderDialog>()
        {
            { x => x.Project, project },
        };

        var dialog = await _dialogService.ShowAsync<PickTestFolderDialog>("Select folder", parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestSuiteFolder folder)
        {
            return folder;
        }
        return null;
    }

    /// <summary>
    /// Deletes a folder
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public async Task DeleteFolderByIdAsync(long folderId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testSuiteManager.DeleteTestSuiteFolderByIdAsync(principal, folderId);
    }

    /// <summary>
    /// Deletes a test suite
    /// </summary>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    public async Task DeleteTestSuiteByIdAsync(long testSuiteId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testSuiteManager.DeleteTestSuiteByIdAsync(principal, testSuiteId);
    }

    /// <summary>
    /// Adds a test suite
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<TestSuite> AddTestSuiteAsync(long? teamId, long? projectId, string name, string? ciCdSystem, string? ciCdRef = null)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.AddTestSuiteAsync(principal, teamId, projectId, name, ciCdSystem, ciCdRef);
    }

    /// <summary>
    /// Saves a test suite
    /// </summary>
    /// <param name="suite"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SaveTestSuiteAsync(TestSuite suite)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testSuiteManager.UpdateTestSuiteAsync(principal, suite);
    }

    public async Task<PagedResult<TestSuite>> GetTestSuitesAsync(long? teamId, long? projectId, int offset = 0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.SearchTestSuitesAsync(principal, new SearchQuery
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
        var principal = await GetUserClaimsPrincipalAsync();
        var fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, searchTestQuery.ProjectId, FieldTarget.TestCase);

        var filters = TestCaseFilterSpecificationBuilder.From(searchTestQuery, fields);

        filters = [new FilterByTenant<TestCase>(tenantId), .. filters];
        return await _testCaseRepo.SearchTestCasesAsync(searchTestQuery.Offset, searchTestQuery.Count, filters);
        //return await _testCaseRepo.SearchTestCasesAsync(tenantId, searchTestQuery);
    }

    internal async Task<TestSuite?> GetTestSuiteByIdAsync(long testSuiteId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.GetTestSuiteByIdAsync(tenantId, testSuiteId);
    }

    internal async Task<TestSuiteFolder?> GetTestSuiteFolderByIdAsync(long folderId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _testCaseRepo.GetTestSuiteFolderByIdAsync(tenantId, folderId);
    }
}
