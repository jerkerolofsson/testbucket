using TestBucket.Components.Tenants;

namespace TestBucket.Components.Tests.Services;


internal interface ITestBrowserObserver
{
    Task OnTestDeletedAsync(TestCase testCase);
}


internal class TestCaseEditorService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;
    private readonly List<ITestBrowserObserver> _observers = new();

    public event EventHandler<TestCase>? TestCaseSaved;

    public TestCaseEditorService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
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
        foreach(var observer in _observers)
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
        TestCaseSaved?.Invoke(this,testCase);

        // observer.OnTestCaseFolderChangedAsync
        // observer...
    }
}
