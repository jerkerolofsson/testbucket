using MudBlazor;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Compiler;

namespace TestBucket.Components.Tests.Services;

internal class TestCaseEditorController : TenantBaseService
{
    private readonly IDialogService _dialogService;
    private readonly IStateService _stateService;
    private readonly ITestRunManager _testRunManager;
    private readonly ITestCaseManager _testCaseManager;
    private readonly IRequirementManager _requirementManager;
    private readonly ITestCompiler _testCompiler;
    private readonly ITestEnvironmentManager _testEnvironmentManager;

    public TestCaseEditorController(
        ITestCaseManager testCaseManager,
        ITestRunManager testRunManager,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService,
        IStateService stateService,
        IRequirementManager requirementManager,
        ITestCompiler testComplier,
        ITestEnvironmentManager testEnvironmentManager) : base(authenticationStateProvider)
    {
        _testCaseManager = testCaseManager;
        _testRunManager = testRunManager;
        _dialogService = dialogService;
        _stateService = stateService;
        _requirementManager = requirementManager;
        _testCompiler = testComplier;
        _testEnvironmentManager = testEnvironmentManager;
    }

    /// <summary>
    /// Compiles a preview for a test case with a specific context
    /// </summary>
    /// <param name="testCase"></param>
    /// <param name="context"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<string?> CompileTestCaseRunPreviewAsync(TestCase testCase, long runId, string? text)
    {
        if (text is not null && testCase.TestProjectId is not null && testCase.TeamId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();

            var run = await _testRunManager.GetTestRunByIdAsync(principal, runId);
            if(run?.TestProjectId is null)
            {
                return null;
            }

            var context = new TestExecutionContext()
            {
                TestCaseId = testCase.Id,
                ProjectId = run.TestProjectId.Value,
                TestRunId = run.Id,
                TeamId = testCase.TeamId.Value,
                TestEnvironmentId = run.TestEnvironmentId
            };

            return await CompilePreviewAsync(testCase, context, text);
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
    public async Task<string?> CompilePreviewAsync(TestCase testCase, TestExecutionContext context, string? text)
    {
        if (text is not null && testCase.TestProjectId is not null && testCase.TeamId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();

            await _testCompiler.ResolveVariablesAsync(principal, context);
            return await _testCompiler.CompileAsync(principal, context, text);
        }
        return text;
    }
    public async Task<string?> CompilePreviewAsync(TestCase testCase, string? text)
    {
        if (text is not null && testCase.TestProjectId is not null && testCase.TeamId is not null)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            var defaultEnvironment = await _testEnvironmentManager.GetDefaultTestEnvironmentAsync(principal, testCase.TestProjectId.Value);

            var context = new TestExecutionContext()
            {
                TestCaseId = testCase.Id,
                ProjectId = testCase.TestProjectId.Value,
                TestRunId = 0,
                TeamId = testCase.TeamId.Value,
                TestEnvironmentId = defaultEnvironment?.Id
            };
            return await CompilePreviewAsync(testCase, context, text);
        }
        return text;
    }

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public async Task AddTestCaseAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        testCase.TenantId = await GetTenantIdAsync();
        await _testCaseManager.AddTestCaseAsync(principal, testCase);
    }
    public async Task GenerateAiTestsAsync(TestSuiteFolder? folder, long? testSuiteId)
    {
        var parameters = new DialogParameters<CreateAITestsDialog>
        {
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId}
        };
        var dialog = await _dialogService.ShowAsync<CreateAITestsDialog>("Generate test cases", parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
      
    }


    public async Task<TestCase?> CreateNewTestCaseAsync(TestProject project, TestSuiteFolder? folder, long? testSuiteId, string name = "")
    {
        var parameters = new DialogParameters<AddTestCaseDialog>
        {
            { x => x.Name, name },
            { x => x.Project, project },
            { x => x.Folder, folder },
            { x => x.TestSuiteId, testSuiteId ?? folder?.TestSuiteId}
        };
        var dialog = await _dialogService.ShowAsync<AddTestCaseDialog>("Add test case", parameters, DefaultBehaviors.DialogOptions);
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
    public async Task DeleteTestCaseAsync(TestCase testCase)
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
    public async Task LinkTestCaseToRequirementAsync(TestCase testCase, TestProject? project, Team? team)
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

    /// <summary>
    /// Saves a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SaveTestCaseAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testCaseManager.SaveTestCaseAsync(principal, testCase);
    }

    internal async Task DeleteTestRunAsync(TestRun testRun)
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
    public async Task SaveTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.SaveTestCaseRunAsync(principal, testCaseRun);
    }

    internal async Task DuplicateTestAsync(TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var copy = new TestCase
        {
            Name = testCase.Name + " copy",
            Description = testCase.Description,

            TestSuiteId = testCase.TestSuiteId,
            TestProjectId = testCase.TestProjectId,
            TestSuiteFolderId = testCase.TestSuiteFolderId,
            TeamId = testCase.TeamId,
        };

        // todo: copy fields

        await _testCaseManager.AddTestCaseAsync(principal, copy);
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
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.AddTestCaseRunAsync(principal, testCaseRun);
    }

    internal async Task AddTestRunAsync(TestRun testRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.AddTestRunAsync(principal, testRun);
    }

    internal async Task SaveTestRunAsync(TestRun testRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _testRunManager.SaveTestRunAsync(principal, testRun);
    }

    internal async Task<TestState> GetProjectFinalStateAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetProjectFinalStateAsync(principal, testProjectId);
    }

    internal async Task<TestState> GetProjectInitialStateAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetProjectInitialStateAsync(principal, testProjectId);
    }
}
