using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Dialogs;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Components.Tests.Services;

internal class TestExecutionController : TenantBaseService
{
    private readonly IDialogService _dialogService;
    private readonly TestCaseEditorController _editorController;
    private readonly IUserPreferencesManager _userPreferencesManager;

    /// <summary>
    /// Invoked when a result is set, unless the result is NoRun
    /// </summary>
    public event EventHandler<TestCaseRun>? TestCompleted;

    public TestExecutionController(
        AuthenticationStateProvider authenticationStateProvider,
        TestCaseEditorController testCaseEditor,
        IDialogService dialogService,
        TestCaseEditorController editorController,
        IUserPreferencesManager userPreferencesManager) : base(authenticationStateProvider)
    {
        _dialogService = dialogService;
        _editorController = editorController;
        _userPreferencesManager = userPreferencesManager;
    }

    public async Task RunTestAsync(TestCaseRun testCaseRun)
    {
        var parameters = new DialogParameters<RunTestCaseDialog>()
        {
            { x => x.TestCaseRun, testCaseRun }
        };
        var dialog = await _dialogService.ShowAsync<RunTestCaseDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        await dialog.Result;
    }

    public async Task SetTestCaseRunResultAsync(TestCaseRun testCaseRun, TestResult result)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        testCaseRun.Result = result;

        // Set the state to the final state
        if (testCaseRun.TestProjectId is not null)
        {
            if (testCaseRun.Result != TestResult.NoRun)
            {
                TestState completedState = await _editorController.GetProjectFinalStateAsync(testCaseRun.TestProjectId.Value);
                testCaseRun.State = completedState.Name;
            }
            else
            {
                TestState initialState = await _editorController.GetProjectInitialStateAsync(testCaseRun.TestProjectId.Value);
                testCaseRun.State = initialState.Name;
            }
        }

        // Assign to user if not assigned to anyone
        if(testCaseRun.AssignedToUserName is null)
        {
            testCaseRun.AssignedToUserName = principal.Identity?.Name;
        }

        await _editorController.SaveTestCaseRunAsync(testCaseRun);

        var userPreferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
        if (result == TestResult.Failed && userPreferences.ShowFailureMessageDialogWhenFailingTestCaseRun)
        {
            await ShowTestCaseRunFailureDialogAsync(testCaseRun);
        }

        if(result != TestResult.NoRun && userPreferences.AdvanceToNextNotCompletedTestWhenSettingResult)
        {
            TestCompleted?.Invoke(this, testCaseRun);
        }
    }


    public async Task SetTestCaseRunMessageAsync(TestCaseRun testCaseRun, string message)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        testCaseRun.Message = message;
        await _editorController.SaveTestCaseRunAsync(testCaseRun);
    }

    public async Task ShowTestCaseRunFailureDialogAsync(TestCaseRun testCaseRun)
    {
        var parameters = new DialogParameters<TestCaseRunFailureDialog>()
        {
            { x => x.TestCaseRun, testCaseRun }
        };
        var dialog = await _dialogService.ShowAsync<TestCaseRunFailureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        await dialog.Result;
    }
}
