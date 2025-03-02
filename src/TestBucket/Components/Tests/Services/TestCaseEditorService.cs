namespace TestBucket.Components.Tests.Services;

internal class TestCaseEditorService : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepo;

    public event EventHandler<TestCase>? TestCaseSaved;

    public TestCaseEditorService(ITestCaseRepository testCaseRepo,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _testCaseRepo = testCaseRepo;
    }

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
            throw new InvalidOperationException("TenantId mismatch");
        }
        await _testCaseRepo.UpdateTestCaseAsync(testCase);
        TestCaseSaved?.Invoke(this,testCase);
    }
}
