
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Dialogs;
using TestBucket.Contracts.Identity;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Automation.Hybrid;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Components.Tests.Services;

internal class TestRunCreationController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _testCaseEditor;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IDialogService _dialogService;
    private readonly IMarkdownAutomationRunner _markdownAutomationRunner;
    private readonly ITestEnvironmentManager _testEnvironmentManager;
    private readonly IPipelineProjectManager _pipelineProjectManager;
    private readonly ITenantManager _tenantManager;

    public TestRunCreationController(
        AuthenticationStateProvider authenticationStateProvider,
        TestCaseEditorController testCaseEditor,
        IDialogService dialogService,
        ITestCaseRepository testCaseRepository,
        IMarkdownAutomationRunner markdownAutomationRunner,
        ITestEnvironmentManager testEnvironmentManager,
        AppNavigationManager appNavigationManager,
        IPipelineProjectManager pipelineProjectManager,
        ITenantManager tenantManager) : base(authenticationStateProvider)
    {
        _testCaseEditor = testCaseEditor;
        _dialogService = dialogService;
        _testCaseRepository = testCaseRepository;
        _markdownAutomationRunner = markdownAutomationRunner;
        _testEnvironmentManager = testEnvironmentManager;
        _appNavigationManager = appNavigationManager;
        _pipelineProjectManager = pipelineProjectManager;
        _tenantManager = tenantManager;
    }

    public async Task<TestRun?> CreateTestRunAsync(TestSuite testSuite, long[]testCaseIds, bool startAutomation)
    {
        var testRun = await CreateTestRunAsync(testSuite);
        if (testRun is not null)
        {
            // Add all manual test cases
            foreach (var testCaseId in testCaseIds)
            {
                await AddTestCaseToRunAsync(testRun, testCaseId);
            }

            // Start automation pipeline
            if(startAutomation && !string.IsNullOrEmpty(testSuite.CiCdSystem))
            {
                await StartAutomationAsync(testRun, testSuite.Variables, testSuite?.Id);
            }
        }
        return testRun;
    }

    /// <summary>
    /// Starts an automation test with the specified test suite
    /// </summary>
    /// <param name="testRun"></param>
    /// <param name="variables"></param>
    /// <param name="testSuiteId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task StartAutomationAsync(TestRun testRun, Dictionary<string,string>? variables, long? testSuiteId)
    {
        if (testRun.CiCdSystem is null)
        {
            throw new ArgumentException("CI/CD system not set");
        }
        if (testRun.ExternalSystemId is null)
        {
            throw new ArgumentException("CI/CD integration config not set");
        }
        if (testRun.CiCdRef is null)
        {
            throw new ArgumentException("CI/CD ref not set");
        }
        if (testRun.TestProjectId is null)
        {
            throw new ArgumentException("project not set");
        }
        if (testRun.TeamId is null)
        {
            throw new ArgumentException("team not set");
        }

        variables ??= new();

        TestExecutionContext context = new TestExecutionContext
        {
            TestSuiteId = testSuiteId,
            TenantId = testRun.TenantId,
            TestRunId = testRun.Id,
            ProjectId = testRun.TestProjectId.Value,
            CiCdRef = testRun.CiCdRef,
            CiCdSystem = testRun.CiCdSystem,
            CiCdExternalSystemId = testRun.ExternalSystemId,
            TeamId = testRun.TeamId.Value,
            TestEnvironmentId = testRun.TestEnvironmentId,
            Variables = variables
        };

        var principal = await GetUserClaimsPrincipalAsync();
        await _pipelineProjectManager.CreatePipelineAsync(principal, context);
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
        var dialog = await _dialogService.ShowAsync<CreateTestRunDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if(result?.Data is TestRun testRun)
        {
            return testRun;
        }
        return null;
    }

    public async Task<TestRun?> CreateTestRunAsync(TestSuite testSuite)
    {
        if(testSuite.TestProjectId is null)
        {
            return null;
        }

        // Default test environment
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<CreateTestRunDialog>()
        {
            { x => x.Name, testSuite.Name },
            { x => x.TestSuite, testSuite },
            { x => x.TestProjectId, testSuite.TestProjectId }
        };
        var dialog = await _dialogService.ShowAsync<CreateTestRunDialog>(null, parameters);
        var result = await dialog.Result;
        if (result?.Data is TestRun testRun)
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

    internal async Task EvalMarkdownCodeAsync(TestCase test, string language, string code)
    {
        ArgumentNullException.ThrowIfNull(test.TestProjectId);

        var principal = await GetUserClaimsPrincipalAsync();

        if (await _markdownAutomationRunner.SupportsLanguageAsync(principal, language))
        {
            if (test?.TeamId is not null)
            {
                var context = new TestExecutionContext
                {
                    ProjectId = test.TestProjectId.Value,
                    TeamId = test.TeamId.Value,
                    TestRunId = null,
                    TestCaseId = test.Id,
                    TestEnvironmentId = null // todo: default envi
                };

                var result = await _markdownAutomationRunner.EvalAsync(principal, context, language, code);
                if (result is not null)
                {

                }
            }
        }
        //else
        //{
        //    // Create an empty test and let the user run it
        //    var run = await CreateTestRunAsync(test.Name, test.TestProjectId.Value, [test.Id]);
        //    if (run is not null)
        //    {
        //        _appNavigationManager.NavigateTo(run);
        //    }
        //}
    }

    internal async Task RunMarkdownCodeAsync(TestCase test, string language, string code)
    {
        ArgumentNullException.ThrowIfNull(test.TestProjectId);

        var principal = await GetUserClaimsPrincipalAsync();

        if(await _markdownAutomationRunner.SupportsLanguageAsync(principal, language))
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
                    TestEnvironmentId = run.TestEnvironmentId
                };

                var result = await _markdownAutomationRunner.RunAsync(principal, context, language, code);
                if (result is not null)
                {

                }
                _appNavigationManager.NavigateTo(run);
            }
        }
        //else
        //{
        //    // Create an empty test and let the user run it
        //    var run = await CreateTestRunAsync(test.Name, test.TestProjectId.Value, [test.Id]);
        //    if (run is not null)
        //    {
        //        _appNavigationManager.NavigateTo(run);
        //    }
        //}
    }

}
