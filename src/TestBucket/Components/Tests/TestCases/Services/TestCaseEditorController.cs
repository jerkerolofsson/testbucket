using Mediator;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Tests.TestCases.Dialogs;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.TestAccounts.Allocation;
using TestBucket.Domain.Testing.Compiler;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestCases.Services;

internal class TestCaseEditorController : TenantBaseService, IAsyncDisposable
{
    private readonly IDialogService _dialogService;
    private readonly IStateService _stateService;
    private readonly ITestRunManager _testRunManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IRequirementManager _requirementManager;
    private readonly ITestCompiler _testCompiler;
    private readonly ITestEnvironmentManager _testEnvironmentManager;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedStrings> _loc;


    public TestCaseEditorController(
        ITestCaseManager testCaseManager,
        ITestRunManager testRunManager,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        IStateService stateService,
        IRequirementManager requirementManager,
        ITestCompiler testComplier,
        ITestEnvironmentManager testEnvironmentManager,
        IMediator mediator,
        ITestSuiteManager testSuiteManager,
        IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _testCaseManager = testCaseManager;
        _testRunManager = testRunManager;
        _dialogService = dialogService;
        _stateService = stateService;
        _requirementManager = requirementManager;
        _testCompiler = testComplier;
        _testEnvironmentManager = testEnvironmentManager;
        _mediator = mediator;
        _testSuiteManager = testSuiteManager;
        _loc = loc;
    }

    public async ValueTask<TestExecutionContext?> CompileAsync(CompilationOptions options, List<CompilerError> errors, CancellationToken cancellationToken = default)
    {
        var testCase = options.TestCase;
        if (testCase.TestProjectId is not null && testCase.TeamId is not null && testCase.TenantId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();

            TestRun? testRun;
            TestEnvironment? testEnvironment = null;
            if(options.TestRunId is not null)
            {
                testRun = await _testRunManager.GetTestRunByIdAsync(principal, options.TestRunId.Value);
                if(testRun?.TestEnvironmentId is not null)
                {
                    testEnvironment = await _testEnvironmentManager.GetTestEnvironmentByIdAsync(principal, testRun.TestEnvironmentId.Value);
                }
            }
            if (testEnvironment is null)
            {
                var defaultEnvironment = await _testEnvironmentManager.GetDefaultTestEnvironmentAsync(principal, testCase.TestProjectId.Value);
                testEnvironment = defaultEnvironment;
            }

            var context = new TestExecutionContext()
            {
                Guid = options.ContextGuid,
                TestCaseId = testCase.Id,
                TestSuiteId = testCase.TestSuiteId,
                ProjectId = testCase.TestProjectId.Value,
                TestRunId = options.TestRunId,
                TeamId = testCase.TeamId.Value,
                TestEnvironmentId = testEnvironment?.Id,
                Dependencies = []
            };

            if(options.AllocateResources)
            {
                if(testCase.Dependencies is not null)
                {
                    context.Dependencies = [.. testCase.Dependencies];
                }

                var suite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, testCase.TestSuiteId);
                if(suite?.Dependencies is not null) 
                {
                    context.Dependencies.AddRange(suite.Dependencies);
                }
            }

            await _testCompiler.ResolveVariablesAsync(principal, context, cancellationToken);
            var result = await _testCompiler.CompileAsync(principal, context, options.Text);

            errors.Clear();
            errors.AddRange(context.CompilerErrors);

            if (options.ReleaseResourceDirectly)
            {
                await ReleaseResourcesAsync(context.Guid, testCase.TenantId);
            }

            context.CompiledText = result;
            return context;
        }
        return null;
    }

    public async ValueTask ReleaseResourcesAsync(string guid, string tenantId)
    {
        await _mediator.Send(new ReleaseResourcesRequest(guid, tenantId));
        await _mediator.Send(new ReleaseAccountsRequest(guid, tenantId));
    }

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public async ValueTask AddTestCaseAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        testCase.TenantId = await GetTenantIdAsync();
        await _testCaseManager.AddTestCaseAsync(principal, testCase);
    }
    public async ValueTask GenerateAiTestsAsync(TestSuiteFolder? folder, long? testSuiteId)
    {
        var parameters = new DialogParameters<CreateAITestsDialog>
        {
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId}
        };
        var dialog = await _dialogService.ShowAsync<CreateAITestsDialog>("Generate test cases", parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
    }
    public async ValueTask<TestCase?> CreateNewTemplateAsync(TestProject project, TestSuiteFolder? folder, long? testSuiteId, string name = "")
    {
        var parameters = new DialogParameters<AddTestCaseDialog>
        {
            { x => x.Name, name },
            { x => x.IsTemplate, true },
            { x => x.Project, project },
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId }
        };
        var dialog = await _dialogService.ShowAsync<AddTestCaseDialog>(_loc["new-test-template"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestCase testCase)
        {
            testCase.Description = """

                A test case using this template will contain all the text from the template, and replace the Body below 
                with it's own text

                @Body

                Text in the template can also be below the body.

                > A test case using this template need to define a template directive in it's own description body.

                """;

            return testCase;
        }
        return null;
    }
    public async ValueTask<TestCase?> CreateNewSharedStepsAsync(TestProject project, TestSuiteFolder? folder, long? testSuiteId, string name = "")
    {
        var parameters = new DialogParameters<AddTestCaseDialog>
        {
            { x => x.Name, name },
            { x => x.IsTemplate, true },
            { x => x.Project, project },
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId }
        };
        var dialog = await _dialogService.ShowAsync<AddTestCaseDialog>(_loc["new-shared-steps"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestCase testCase)
        {
            return testCase;
        }
        return null;
    }
    public async ValueTask<TestCase?> CreateNewTestCaseAsync(TestProject project, TestSuiteFolder? folder, long? testSuiteId, string name = "")
    {
        var parameters = new DialogParameters<AddTestCaseDialog>
        {
            { x => x.Name, name },
            { x => x.Project, project },
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId }
        };
        var dialog = await _dialogService.ShowAsync<AddTestCaseDialog>(_loc["new-test"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestCase testCase)
        {
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
    public async ValueTask DeleteTestCaseAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testCaseManager.DeleteTestCaseAsync(principal, testCase);
    }


    /// <summary>
    /// Lets the user select a requirement and creates a link to the test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <param name="project"></param>
    /// <param name="team"></param>
    /// <returns></returns>
    public async ValueTask LinkTestCaseToRequirementAsync(TestCase testCase, TestProject? project, Team? team)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<PickRequirementDialog>()
        {
            { x => x.Project, project },
            { x => x.Team, team },
        };
        var dialog = await _dialogService.ShowAsync<PickRequirementDialog>(_loc["select-requirement"], parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Requirement requirement)
        {
            await _requirementManager.AddRequirementLinkAsync(principal, requirement, testCase);
        }
    }

    public async ValueTask LinkTestCaseToRequirementAsync(long testCaseId, Requirement requirement)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _requirementManager.AddRequirementLinkAsync(principal, requirement, testCaseId);
    }

    public async ValueTask LinkTestCaseToRequirementAsync(TestCase testCase, Requirement requirement)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _requirementManager.AddRequirementLinkAsync(principal, requirement, testCase);
    }

    /// <summary>
    /// Saves a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async ValueTask SaveTestCaseAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testCaseManager.SaveTestCaseAsync(principal, testCase);
    }

    internal async ValueTask DeleteTestRunAsync(TestRun testRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.DeleteTestRunAsync(principal, testRun);
    }

    /// <summary>
    /// Saves a test case run
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async ValueTask SaveTestCaseRunAsync(TestCaseRun testCaseRun, bool informObservers = true)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.SaveTestCaseRunAsync(principal, testCaseRun, informObservers);
    }

    /// <summary>
    /// Duplicates a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    internal async ValueTask DuplicateTestAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        await _testCaseManager.DuplicateTestCaseAsync(principal, testCase);
       
    }

    internal async ValueTask EditTestCaseAutomationLinkAsync(TestCase testCase)
    {
        var parameters = new DialogParameters<EditTestCaseAutomationLinkDialog>
        {
            { x => x.TestCase, testCase },
        };
        var dialog = await _dialogService.ShowAsync<EditTestCaseAutomationLinkDialog>(null, parameters);
        var result = await dialog.Result;
    }

    internal async ValueTask AddTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.AddTestCaseRunAsync(principal, testCaseRun);
    }

    internal async ValueTask AddTestRunAsync(TestRun testRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        testRun.Open = true;
        await _testRunManager.AddTestRunAsync(principal, testRun);
    }

    internal async ValueTask SaveTestRunAsync(TestRun testRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.SaveTestRunAsync(principal, testRun);
    }

    internal async ValueTask<TestState> GetProjectFinalStateAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetProjectFinalStateAsync(principal, testProjectId);
    }

    internal async ValueTask<TestState> GetProjectInitialStateAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetProjectInitialStateAsync(principal, testProjectId);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
