
using Mediator;

using TestBucket.Components.Tests.Dialogs;
using TestBucket.Components.Tests.TestCases.Models;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Dialogs;
using TestBucket.Domain.Automation.Hybrid;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.Testing.TestSuites.Search;
using TestBucket.Domain.TestResources.Allocation;

namespace TestBucket.Components.Tests.TestRuns.Controllers;

internal class TestRunController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly ITestRunManager _testRunManager;

    public TestRunController(AuthenticationStateProvider authenticationStateProvider, AppNavigationManager appNavigationManager, ITestRunManager testRunManager) : base(authenticationStateProvider)
    {
        _appNavigationManager = appNavigationManager;
        _testRunManager = testRunManager;
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
