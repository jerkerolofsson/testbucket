

using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestSuites.Dialogs;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestCases.Search;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.Testing.TestSuites.Search;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Services;

internal class TestSuiteController : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public TestSuiteController(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider,
        IFieldDefinitionManager fieldDefinitionManager,
        ITestSuiteManager testSuiteManager,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc,
        ITestCaseManager testCaseManager) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
        _fieldDefinitionManager = fieldDefinitionManager;
        _testSuiteManager = testSuiteManager;
        _dialogService = dialogService;
        _loc = loc;
        _testCaseManager = testCaseManager;
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

        var dialog = await _dialogService.ShowAsync<PickTestFolderDialog>(_loc["select-folder"], parameters, DefaultBehaviors.DialogOptions);
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
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.TestSuite, PermissionLevel.Delete))
        {
            return;
        }

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _testSuiteManager.DeleteTestSuiteFolderByIdAsync(principal, folderId);
        }
    }

    /// <summary>
    /// Deletes a test suite
    /// </summary>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    public async Task DeleteTestSuiteByIdAsync(long testSuiteId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        if(!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.TestSuite, PermissionLevel.Delete))
        {
            return;
        }

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _testSuiteManager.DeleteTestSuiteByIdAsync(principal, testSuiteId);
        }
    }

    /// <summary>
    /// Adds a test suite
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<TestSuite?> AddTestSuiteAsync(long? teamId, long? projectId, string name, string? ciCdSystem, string? ciCdRef = null)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.TestSuite, PermissionLevel.Write))
        {
            return null;
        }

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
        if (!await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.TestSuite, PermissionLevel.Write))
        {
            return;
        }

        var principal = await GetUserClaimsPrincipalAsync();
        await _testSuiteManager.UpdateTestSuiteAsync(principal, suite);
    }
    public async Task<PagedResult<TestSuite>> GetTestSuitesInFolderAsync(long? teamId, long? projectId, long folderId, int offset = 0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.SearchTestSuitesAsync(principal, new SearchTestSuiteQuery
        {
            FolderId = folderId,
            TeamId = teamId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }

    public async Task<PagedResult<TestSuite>> GetRootTestSuitesAsync(long? teamId, long? projectId, int offset = 0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.SearchTestSuitesAsync(principal, new SearchTestSuiteQuery
        {
            RootFolders = true,
            TeamId = teamId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }
    public async Task<PagedResult<TestSuite>> GetTestSuitesAsync(long? teamId, long? projectId, int offset = 0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.SearchTestSuitesAsync(principal, new SearchTestSuiteQuery
        {
            TeamId = teamId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }

    internal async Task<PagedResult<TestCase>> SearchTestCasesAsync(SearchTestQuery searchTestQuery)
    {
        //var tenantId = await GetTenantIdAsync();
        var principal = await GetUserClaimsPrincipalAsync();
        //var fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, searchTestQuery.ProjectId, FieldTarget.TestCase);

        //var filters = TestCaseFilterSpecificationBuilder.From(searchTestQuery);

        //filters = [new FilterByTenant<TestCase>(tenantId), .. filters];
        //return await _testCaseRepo.SearchTestCasesAsync(searchTestQuery.Offset, searchTestQuery.Count, filters);
        return await _testCaseManager.SearchTestCasesAsync(principal, searchTestQuery);
    }

    internal async Task<TestSuite?> GetTestSuiteByIdAsync(long testSuiteId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.GetTestSuiteByIdAsync(principal, testSuiteId);
        //return await _testCaseRepo.GetTestSuiteByIdAsync(tenantId, testSuiteId);
    }

    internal async Task<TestSuiteFolder?> GetTestSuiteFolderByIdAsync(long folderId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testSuiteManager.GetTestSuiteFolderByIdAsync(principal, folderId);

        //var tenantId = await GetTenantIdAsync();

        //_testSuiteManager.GetTestSuiteByNameAsync
        //return await _testCaseRepo.GetTestSuiteFolderByIdAsync(tenantId, folderId);
    }
}
