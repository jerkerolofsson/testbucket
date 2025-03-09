

using TestBucket.Components.Tests.Controls;
using TestBucket.Components.Tests.Dialogs;

namespace TestBucket.Components.Tests.Services;

internal class TestRunCreationService : TenantBaseService
{
    private readonly TestCaseEditorService _testCaseEditor;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IDialogService _dialogService;

    public TestRunCreationService(
        AuthenticationStateProvider authenticationStateProvider,
        TestCaseEditorService testCaseEditor,
        IDialogService dialogService,
        ITestCaseRepository testCaseRepository) : base(authenticationStateProvider)
    {
        _testCaseEditor = testCaseEditor;
        _dialogService = dialogService;
        _testCaseRepository = testCaseRepository;
    }

    public async Task<TestRun?> CreateTestRunAsync(long projectId, long testCaseId)
    {
        var testRun = await CreateTestRunAsync(projectId);
        if(testRun is not null)
        {
            // Add test case to run
            await AddTestCaseToRunAsync(testRun, testCaseId);

        }
        return testRun;
    }

    public async Task<TestRun?> CreateTestRunAsync(long projectId)
    {
        var parameters = new DialogParameters<CreateTestRunDialog>()
        {
            { x => x.TestProjectId, projectId }
        };
        var dialog = await _dialogService.ShowAsync<CreateTestRunDialog>(null, parameters);
        var result = await dialog.Result;
        if(result?.Data is TestRun testRun)
        {
            return testRun;
        }
        return null;
    }

    public async Task AddTestCaseToRunAsync(TestRun run, long testCaseId)
    {
        var tenantId = await GetTenantIdAsync();
        TestCase? testCase = await _testCaseRepository.GetTestCaseByIdAsync(tenantId, testCaseId);
        if (testCase is null)
        {
            throw new ArgumentException("Test case with the specified ID was not found!");
        }
        await AddTestCaseToRunAsync(run, testCase);
    }

    internal async Task AddTestCaseToRunAsync(TestRun run, TestCase testCase)
    {
        var tenantId = await GetTenantIdAsync();

        if (run.TestProjectId is null)
        {
            throw new ArgumentException("TestRun must belong to a project!");
        }    

        var testCaseRun = new TestCaseRun
        {
            Name = testCase.Name,
            TestCaseId = testCase.Id,
            TestRunId = run.Id,
            TestProjectId = run.TestProjectId.Value,
            TenantId = tenantId,
            State = "Not Started",
        };

        await _testCaseEditor.AddTestCaseRunAsync(testCaseRun);

        // Todo: Copy traits from test case to test case run

        // Todo: Assign?

    }
}
