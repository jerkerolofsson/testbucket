
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Automation.Services;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;

namespace TestBucket.Components.Tests.Services;

internal class TestRunCreationController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _testCaseEditor;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IDialogService _dialogService;
    private readonly IMarkdownAutomationRunner _markdownAutomationRunner;
    private readonly ITestEnvironmentManager _testEnvironmentManager;

    public TestRunCreationController(
        AuthenticationStateProvider authenticationStateProvider,
        TestCaseEditorController testCaseEditor,
        IDialogService dialogService,
        ITestCaseRepository testCaseRepository,
        IMarkdownAutomationRunner markdownAutomationRunner,
        ITestEnvironmentManager testEnvironmentManager,
        AppNavigationManager appNavigationManager) : base(authenticationStateProvider)
    {
        _testCaseEditor = testCaseEditor;
        _dialogService = dialogService;
        _testCaseRepository = testCaseRepository;
        _markdownAutomationRunner = markdownAutomationRunner;
        _testEnvironmentManager = testEnvironmentManager;
        _appNavigationManager = appNavigationManager;
    }

    public async Task<TestRun?> CreateTestRunAsync(string name, long projectId, long[] testCaseIds)
    {
        var testRun = await CreateTestRunAsync(name, projectId);
        if(testRun is not null)
        {
            // Add test case to run
            foreach (var testCaseId in testCaseIds)
            {
                await AddTestCaseToRunAsync(testRun, testCaseId);
            }

        }
        return testRun;
    }

    /// <summary>
    /// Creates a new test run by displaying a dialog
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<TestRun?> CreateTestRunAsync(string name, long projectId)
    {
        // Default test environment
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<CreateTestRunDialog>()
        {
            { x => x.Name, name },
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

    /// <summary>
    /// Adds a test case to a run
    /// </summary>
    /// <param name="run"></param>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Adds a test case to a run
    /// </summary>
    /// <param name="run"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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

        // Todo: Assign?

    }

    internal async Task RunMarkdownCodeAsync(TestCase test, string language, string code)
    {
        ArgumentNullException.ThrowIfNull(test.TestProjectId);

        var principal = await GetUserClaimsPrincipalAsync();

        if(_markdownAutomationRunner.SupportsLanguage(language))
        {
            var run = await CreateTestRunAsync(test.Name, test.TestProjectId.Value, []);

            if (run is not null)
            {
                ArgumentNullException.ThrowIfNull(run.TestProjectId);
                ArgumentNullException.ThrowIfNull(run.TeamId);

                var context = new TestExecutionContext
                {
                    ProjectId = run.TestProjectId.Value,
                    TeamId = run.TeamId.Value,
                    TestRunId = run.Id,
                    TestCaseId = test.Id,
                };
                await _markdownAutomationRunner.RunAsync(principal, context, language, code);
                _appNavigationManager.NavigateTo(run);
            }
        }
        else
        {
            // Create an empty test and let the user run it
            var run = await CreateTestRunAsync(test.Name, test.TestProjectId.Value, [test.Id]);
            if (run is not null)
            {
                _appNavigationManager.NavigateTo(run);
            }
        }
    }

}
