using MudBlazor;

using TestBucket.Components.Tenants;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Components.Tests.Services;

internal interface ITestBrowserObserver
{
    Task OnTestCreatedAsync(TestCase testCase);
    Task OnTestDeletedAsync(TestCase testCase);
    Task OnTestSavedAsync(TestCase testCase);
}


internal class TestCaseEditorService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;
    private readonly List<ITestBrowserObserver> _observers = new();
    private readonly IDialogService _dialogService;

    public TestCaseEditorService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(ITestBrowserObserver observer) => _observers.Add(observer);

    /// <summary>
    /// Remopves an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(ITestBrowserObserver observer) => _observers.Remove(observer);

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
            foreach (var observer in _observers.ToList())
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
        foreach (var observer in _observers.ToList())
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
        foreach (var observer in _observers.ToList())
        {
            await observer.OnTestSavedAsync(testCase);
        }

        // observer.OnTestCaseFolderChangedAsync
        // observer...
    }

    internal async Task DeleteTestRunByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        await _testCaseRepo.DeleteTestRunByIdAsync(tenantId, id);
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
}
