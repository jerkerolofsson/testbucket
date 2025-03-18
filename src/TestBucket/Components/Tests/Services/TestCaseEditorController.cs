using MudBlazor;

using TestBucket.Components.Tenants;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.States;

namespace TestBucket.Components.Tests.Services;

internal interface ITestCaseObserver
{
    Task OnTestCreatedAsync(TestCase testCase);
    Task OnTestDeletedAsync(TestCase testCase);
    Task OnTestSavedAsync(TestCase testCase);
}

internal interface ITestRunObserver
{
    Task OnRunCreatedAsync(TestRun testRun);
    Task OnRunDeletedAsync(TestRun testRun);
}


internal class TestCaseEditorController : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;
    private readonly List<ITestCaseObserver> _testCaseObservers = new();
    private readonly List<ITestRunObserver> _testRunObservers = new();
    private readonly IDialogService _dialogService;
    private readonly IStateService _stateService;

    public TestCaseEditorController(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        IStateService stateService) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
        _dialogService = dialogService;
        _stateService = stateService;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(ITestCaseObserver observer) => _testCaseObservers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(ITestCaseObserver observer) => _testCaseObservers.Remove(observer);

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public async Task AddTestCaseAsync(TestCase testCase)
    {
        testCase.TenantId = await GetTenantIdAsync();
        await _testCaseRepo.AddTestCaseAsync(testCase);
    }
    public async Task<TestCase[]> GenerateAiTestsAsync(TestSuiteFolder? folder, long? testSuiteId)
    {
        var parameters = new DialogParameters<CreateAITestsDialog>
        {
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId}
        };
        var dialog = await _dialogService.ShowAsync<CreateAITestsDialog>("Generate test cases", parameters);
        var result = await dialog.Result;
        if (result?.Data is TestCase[] testCases)
        {
            // Notify observers
            foreach (var testCase in testCases)
            {
                foreach (var observer in _testCaseObservers.ToList())
                {
                    await observer.OnTestCreatedAsync(testCase);
                }
            }

            return testCases;
        }
        return [];
    }
    public async Task<TestCase?> CreateNewTestCaseAsync(TestSuiteFolder? folder, long? testSuiteId)
    {
        var parameters = new DialogParameters<AddTestCaseDialog>
        {
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId}
        };
        var dialog = await _dialogService.ShowAsync<AddTestCaseDialog>("Add test case", parameters);
        var result = await dialog.Result;
        if (result?.Data is TestCase testCase)
        {
            // Notify observers
            foreach (var observer in _testCaseObservers.ToList())
            {
                await observer.OnTestCreatedAsync(testCase);
            }

            return testCase;
        }
        return null;
    }

    /// <summary>
    /// Deletes a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DeleteTestCaseAsync(TestCase testCase)
    {
        var tenantId = await GetTenantIdAsync();
        if (tenantId != testCase.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }
        await _testCaseRepo.DeleteTestCaseByIdAsync(testCase.Id);

        // Notify observers
        foreach (var observer in _testCaseObservers.ToList())
        {
            await observer.OnTestDeletedAsync(testCase);
        }
    }

    /// <summary>
    /// Saves a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SaveTestCaseAsync(TestCase testCase)
    {
        var tenantId = await GetTenantIdAsync();
        if(tenantId != testCase.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }

        //todo
        //var oldTestCAse = await _testCaseRepo.GetTestCaseByIdAsync

        // Todo: detect changed

        await _testCaseRepo.UpdateTestCaseAsync(testCase);

        // Notify observers
        foreach (var observer in _testCaseObservers.ToList())
        {
            await observer.OnTestSavedAsync(testCase);
        }
    }

    internal async Task DeleteTestRunByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteTestRunByIdAsync(tenantId, id);
    }

    /// <summary>
    /// Saves a test case run
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SaveTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        var tenantId = await GetTenantIdAsync();
        if (tenantId != testCaseRun.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }

        await _testCaseRepo.UpdateTestCaseRunAsync(testCaseRun);

        // Notify observers
        //foreach (var observer in _observers.ToList())
        //{
        //    await observer.OnTestSavedAsync(testCase);
        //}
    }

    internal async Task EditTestCaseAutomationLinkAsync(TestCase testCase)
    {
        var parameters = new DialogParameters<EditTestCaseAutomationLinkDialog>
        {
            { x => x.TestCase, testCase },
        };
        var dialog = await _dialogService.ShowAsync<EditTestCaseAutomationLinkDialog>(null, parameters);
        var result = await dialog.Result;
    }

    internal async Task AddTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        var tenantId = await GetTenantIdAsync();
        testCaseRun.TenantId = tenantId;
        if (testCaseRun.Created == default)
        {
            testCaseRun.Created = DateTimeOffset.Now;
        }
        await _testCaseRepo.AddTestCaseRunAsync(testCaseRun);
    }

    internal async Task AddTestRunAsync(TestRun testRun)
    {
        var tenantId = await GetTenantIdAsync();
        testRun.TenantId = tenantId;
        if (testRun.Created == default)
        {
            testRun.Created = DateTimeOffset.Now;
        }
        await _testCaseRepo.AddTestRunAsync(testRun);
    }

    internal async Task<TestState> GetProjectFinalStateAsync(long testProjectId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _stateService.GetProjectFinalStateAsync(tenantId, testProjectId);
    }

    internal async Task<TestState> GetProjectInitialStateAsync(long testProjectId)
    {
        var tenantId = await GetTenantIdAsync();
        return await _stateService.GetProjectInitialStateAsync(tenantId, testProjectId);
    }
}
