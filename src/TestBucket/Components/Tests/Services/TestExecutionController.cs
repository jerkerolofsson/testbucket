using TestBucket.Components.Tests.Dialogs;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Identity;

namespace TestBucket.Components.Tests.Services;

internal class TestExecutionController : TenantBaseService
{
    private readonly ITestCaseRepository _testCaseRepository;
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
        ITestCaseRepository testCaseRepository,
        TestCaseEditorController editorController,
        IUserPreferencesManager userPreferencesManager) : base(authenticationStateProvider)
    {
        _dialogService = dialogService;
        _testCaseRepository = testCaseRepository;
        _editorController = editorController;
        _userPreferencesManager = userPreferencesManager;
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

    private async Task ShowTestCaseRunFailureDialogAsync(TestCaseRun testCaseRun)
    {
        var parameters = new DialogParameters<TestCaseRunFailureDialog>()
        {
            { x => x.TestCaseRun, testCaseRun }
        };
        var dialog = await _dialogService.ShowAsync<TestCaseRunFailureDialog>(null, parameters, new DialogOptions { CloseOnEscapeKey = true });
        await dialog.Result;
    }
}
