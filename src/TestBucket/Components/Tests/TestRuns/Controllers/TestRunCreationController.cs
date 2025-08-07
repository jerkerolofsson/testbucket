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
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.TestResources.Allocation;

namespace TestBucket.Components.Tests.TestRuns.Controllers;

internal class TestRunCreationController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly ITestRunManager _testRunManager;

    private readonly IDialogService _dialogService;
    private readonly IMarkdownAutomationRunner _markdownAutomationRunner;
    private readonly ITestEnvironmentManager _testEnvironmentManager;
    private readonly IPipelineProjectManager _pipelineProjectManager;
    private readonly ITenantManager _tenantManager;
    private readonly IMediator _mediator;
    private readonly ITestCompiler _compiler;

    public TestRunCreationController(
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        IMarkdownAutomationRunner markdownAutomationRunner,
        ITestEnvironmentManager testEnvironmentManager,
        AppNavigationManager appNavigationManager,
        IPipelineProjectManager pipelineProjectManager,
        ITenantManager tenantManager,
        IMediator mediator,
        ITestCompiler compiler,
        ITestCaseManager testCaseManager,
        ITestSuiteManager testSuiteManager,
        ITestRunManager testRunManager) : base(authenticationStateProvider)
    {
        _dialogService = dialogService;
        _markdownAutomationRunner = markdownAutomationRunner;
        _testEnvironmentManager = testEnvironmentManager;
        _appNavigationManager = appNavigationManager;
        _pipelineProjectManager = pipelineProjectManager;
        _tenantManager = tenantManager;
        _mediator = mediator;
        _compiler = compiler;
        _testCaseManager = testCaseManager;
        _testSuiteManager = testSuiteManager;
        _testRunManager = testRunManager;
    }

    public async Task<TestRun?> CreateTestRunAsync(TestSuite testSuite, SearchTestQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var testRun = await CreateTestRunAsync(testSuite);
        if (testRun is not null)
        {
            var testCaseList = new TestCaseList { Query = query };

            // Add all manual test cases
            await foreach (var testCaseId in _testCaseManager.SearchTestCaseIdsAsync(principal, testCaseList.Query))
            {
                await AddTestCaseToRunAsync(testRun, testCaseId);
            }

            // Start automation pipeline
            if (!string.IsNullOrEmpty(testRun.CiCdSystem))
            {
                await StartAutomationAsync(testRun, testSuite.Variables, testSuite);
            }
        }
        return testRun;
    }

    public async Task<TestRun?> CreateTestRunAsync(TestSuite testSuite, long[]testCaseIds, bool startAutomation)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var testRun = await CreateTestRunAsync(testSuite);
        if (testRun is not null)
        {
            // Add test cases
            foreach(var testCaseId in testCaseIds)
            {
                await AddTestCaseToRunAsync(testRun, testCaseId);
            }

            // Start automation pipeline
            if(startAutomation && !string.IsNullOrEmpty(testRun.CiCdSystem))
            {
                await StartAutomationAsync(testRun, testSuite.Variables, testSuite);
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
    public async Task StartAutomationAsync(TestRun testRun, Dictionary<string,string>? variables, TestSuite? testSuite)
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
            Guid = Guid.NewGuid().ToString(),
            TestSuiteId = testSuite?.Id,
            TestSuiteName = testSuite?.Name,
            CiCdWorkflow = testSuite?.CiCdWorkflow,

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
            { x => x.Name, DateTime.Now.ToString("yyyy-MM-dd") + " - " + testSuite.Name },
            { x => x.TestSuite, testSuite },
            { x => x.TestProjectId, testSuite.TestProjectId }
        };
        var dialog = await _dialogService.ShowAsync<CreateTestRunDialog>(null, parameters, DefaultBehaviors.DialogOptions);
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
        var principal = await GetUserClaimsPrincipalAsync();
        TestCase? testCase = await _testCaseManager.GetTestCaseByIdAsync(principal, testCaseId);
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
        var principal = await GetUserClaimsPrincipalAsync();

        await _testRunManager.AddTestCaseRunAsync(principal, run, testCase);
    }

    internal async Task EvalMarkdownCodeAsync(TestCase test, long? testRunId, string language, string code, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(test.TenantId);
        ArgumentNullException.ThrowIfNull(test.TestProjectId);

        var principal = await GetUserClaimsPrincipalAsync();

        if (await _markdownAutomationRunner.SupportsLanguageAsync(principal, language))
        {
            var testSuite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, test.TestSuiteId);
            var testEnvironment = await _testEnvironmentManager.GetDefaultTestEnvironmentAsync(principal, test.TestProjectId.Value);

            if(testRunId is not null)
            {
                var testRun = await _testRunManager.GetTestRunByIdAsync(principal, testRunId.Value);
                if(testRun?.TestEnvironmentId is not null)
                {
                    testEnvironment = await _testEnvironmentManager.GetTestEnvironmentByIdAsync(principal, testRun.TestEnvironmentId.Value);
                }
            }

            if (test?.TeamId is not null && testSuite is not null)
            {
                var context = new TestExecutionContext
                {
                    Guid = Guid.NewGuid().ToString(),
                    ProjectId = test.TestProjectId.Value,
                    TeamId = test.TeamId.Value,
                    TestRunId = testRunId,
                    TestCaseId = test.Id,
                    TenantId = test.TenantId,

                    TestEnvironmentId = testEnvironment?.Id, 
                    
                    // Defines the resources and accounts that will be locked by the compiler (ResolveVariablesAsync)
                    Dependencies = testSuite.Dependencies
                };

                try
                {
                    await _compiler.ResolveVariablesAsync(principal, context, cancellationToken);
                    await RunCompiledCodeWithResourcesAsync(language, code, context);
                }
                finally
                {
                    // ResolveVariables will lock resource and generate environment variables
                    await _mediator.Send(new ReleaseResourcesRequest(context.Guid, principal.GetTenantIdOrThrow()));
                    await _mediator.Send(new ReleaseAccountsRequest(context.Guid, principal.GetTenantIdOrThrow()));
                }
            }
        }
    }

    /// <summary>
    /// Runs the code with a context containing already locked resources and assigned variables
    /// </summary>
    /// <param name="language"></param>
    /// <param name="code"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task RunCompiledCodeWithResourcesAsync(string language, string code, TestExecutionContext context)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var compiledCode = await _compiler.CompileAsync(principal, context, code);
        var testResult = await _markdownAutomationRunner.EvalAsync(principal, context, language, compiledCode);

        if (testResult is not null)
        {
            var parameters = new DialogParameters<TestRunnerResultDialog>()
            {
                { x => x.TestRunnerResult, testResult }
            };
            var dialog = await _dialogService.ShowAsync<TestRunnerResultDialog>(null, parameters, DefaultBehaviors.DialogOptions);
            await dialog.Result;
        }
    }

    internal async Task RunMarkdownCodeAsync(TestCase test, string language, string code, CancellationToken cancellationToken = default)
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
                    Guid = Guid.NewGuid().ToString(),
                    ProjectId = run.TestProjectId.Value,
                    TeamId = run.TeamId.Value,
                    TestRunId = run.Id,
                    TestCaseId = test.Id,
                    TestEnvironmentId = run.TestEnvironmentId
                };

                try
                {
                    await _compiler.ResolveVariablesAsync(principal, context, cancellationToken);
                    var compiledCode = await _compiler.CompileAsync(principal, context, code);
                    var result = await _markdownAutomationRunner.RunAsync(principal, context, language, compiledCode);
                    if (result is not null)
                    {

                    }
                }
                finally
                {
                    await _mediator.Send(new ReleaseResourcesRequest(context.Guid, principal.GetTenantIdOrThrow()));
                    await _mediator.Send(new ReleaseAccountsRequest(context.Guid, principal.GetTenantIdOrThrow()));
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

    internal async Task<TestRun> DuplicateTestRunAsync(TestRun run, string filter = "")
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _testRunManager.DuplicateTestRunAsync(principal, run, filter);
    }
}
