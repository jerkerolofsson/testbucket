using Mediator;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Tests.TestCases.Dialogs;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Environments;
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
    /// <summary>
    /// Create a guid that is used to lock resources for manual execution with the scoped session of a user
    /// </summary>
    private string? _controllerGuid;
    private string? _tenantId;

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

    /// <summary>
    /// Compiles the description for a test in the UI for a test run when running the test
    /// </summary>
    /// <param name="testCase"></param>
    /// <param name="context"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public async ValueTask<string?> CompileTestCaseRunPreviewAsync(TestCase testCase, long runId, string? text, List<CompilerError> errors)
    {
        if (text is not null && testCase.TestProjectId is not null && testCase.TeamId is not null && testCase.TenantId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();

            var run = await _testRunManager.GetTestRunByIdAsync(principal, runId);
            if (run?.TestProjectId is null)
            {
                return null;
            }

            _tenantId = testCase.TenantId;
            _controllerGuid = principal.Identity?.Name ?? throw new InvalidOperationException("User is not authenticated");

            // Release resources from previous run
            await ReleaseResourcesAsync(_controllerGuid, _tenantId);

            var testSuite = await _testSuiteManager.GetTestSuiteByIdAsync(principal, testCase.TestSuiteId);

            var context = new TestExecutionContext()
            {
                Guid = _controllerGuid,
                TestCaseId = testCase.Id,
                TestSuiteId = testCase.TestSuiteId,
                ProjectId = run.TestProjectId.Value,
                TestRunId = run.Id,
                TeamId = testCase.TeamId.Value,
                TestEnvironmentId = run.TestEnvironmentId,
                Dependencies = [] // Don't lock resources/accounts when it is only a preview
            };

            errors.Clear();
            var result = await CompilePreviewAsync(testCase, context, text, releaseResourcesImmediately: false);
            errors.AddRange(context.CompilerErrors);
            return result;
        }
        return text;
    }

    /// <summary>
    /// Compiles a preview for a test case with a specific context
    /// </summary>
    /// <param name="testCase"></param>
    /// <param name="context"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private async ValueTask<string?> CompilePreviewAsync(
        TestCase testCase, 
        TestExecutionContext context, 
        string? text,
        bool releaseResourcesImmediately = true,
        CancellationToken cancellationToken = default)
    {
        if (text is not null && testCase.TestProjectId is not null && testCase.TeamId is not null && testCase.TenantId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();

            await _testCompiler.ResolveVariablesAsync(principal, context, cancellationToken);
            var result = await _testCompiler.CompileAsync(principal, context, text);

            if (releaseResourcesImmediately)
            {
                await ReleaseResourcesAsync(context.Guid, testCase.TenantId);
            }

            return result;
        }
        return text;
    }
    public async ValueTask<string?> CompilePreviewAsync(
        TestCase testCase, 
        string? text, 
        List<CompilerError> errors, 
        bool releaseResourcesImmediately = true,
        CancellationToken cancellationToken = default)
    {
        if (text is not null && testCase.TestProjectId is not null && testCase.TeamId is not null && testCase.TenantId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            var defaultEnvironment = await _testEnvironmentManager.GetDefaultTestEnvironmentAsync(principal, testCase.TestProjectId.Value);

            var context = new TestExecutionContext()
            {
                Guid = new Guid().ToString(),
                TestCaseId = testCase.Id,
                TestSuiteId = testCase.TestSuiteId,
                ProjectId = testCase.TestProjectId.Value,
                TestRunId = 0,
                TeamId = testCase.TeamId.Value,
                TestEnvironmentId = defaultEnvironment?.Id,
                Dependencies = [] // Don't allocate resources/accounts when it is only a preview
            };
            var result = await CompilePreviewAsync(testCase, context, text, releaseResourcesImmediately, cancellationToken);

            errors.Clear();
            errors.AddRange(context.CompilerErrors);
            return result;
        }
        return text;
    }

    public async ValueTask ReleaseResourcesAsync()
    {
        if (_tenantId is not null && _controllerGuid is not null)
        {
            await ReleaseResourcesAsync(_controllerGuid, _tenantId);
        }
    }

    private async ValueTask ReleaseResourcesAsync(string guid, string tenantId)
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

        var dialog = await _dialogService.ShowAsync<PickRequirementDialog>("Select requirement", parameters, DefaultBehaviors.DialogOptions);
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
    public async ValueTask SaveTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.SaveTestCaseRunAsync(principal, testCaseRun);
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

    public async ValueTask DisposeAsync()
    {
        await ReleaseResourcesAsync();

    }
}
