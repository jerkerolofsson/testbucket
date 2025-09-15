
using TestBucket.Contracts.Localization;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.TestRuns.Controllers;

internal class TestRunController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly ITestRunManager _testRunManager;
    private readonly IAppLocalization _loc;
    private readonly IDialogService _dialogService;

    public TestRunController(
        AuthenticationStateProvider authenticationStateProvider, 
        AppNavigationManager appNavigationManager, 
        ITestRunManager testRunManager, 
        IAppLocalization loc, 
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _appNavigationManager = appNavigationManager;
        _testRunManager = testRunManager;
        _loc = loc;
        _dialogService = dialogService;
    }

    public async Task<TestRun?> GetTestRunByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testRunManager.GetTestRunByIdAsync(principal, id);
    }
    public async Task SaveTestRunAsync(TestRun run)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.SaveTestRunAsync(principal, run);
    }

    internal async Task DeleteTestCaseRunsAsync(IReadOnlyList<TestCaseRun> testCaseRuns)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc.Shared["yes"],
            NoText = _loc.Shared["no"],
            Title = _loc.Shared["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc.Shared["confirm-delete-message"])
        });
        if (result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            foreach (var testCaseRun in testCaseRuns)
            {
                await _testRunManager.DeleteTestCaseRunAsync(principal, testCaseRun);
            }
        }
    }

    internal async Task DeleteTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc.Shared["yes"],
            NoText = _loc.Shared["no"],
            Title = _loc.Shared["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc.Shared["confirm-delete-message"])
        });
        if (result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _testRunManager.DeleteTestCaseRunAsync(principal, testCaseRun);
        }
    }


    internal async Task<PagedResult<TestRun>> GetTestRunsInFolderAsync(long projectId, long folderId, int offset =0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testRunManager.SearchTestRunsAsync(principal, new SearchTestRunQuery
        {
            FolderId = folderId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }

}
