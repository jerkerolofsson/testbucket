using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Components.Tests.TestRuns.Dialogs;
using TestBucket.Components.Tests.TestRuns.ViewModels;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.Services;

internal class TestExecutionController : TenantBaseService
{
    private readonly IDialogService _dialogService;
    private readonly TestCaseEditorController _editorController;
    private readonly IUserPreferencesManager _userPreferencesManager;
    private readonly ITestRunManager _testRunManager;
    private TestCaseRunGridState? _state = null;
    private readonly AppNavigationManager _appNavigationManager;

    /// <summary>
    /// Invoked when the state is changed (e.g. when a the list of items has changed).
    /// </summary>
    public event EventHandler<TestCaseRunGridState>? TestCaseRunGridStateChanged;

    public TestExecutionController(
        AuthenticationStateProvider authenticationStateProvider,
        TestCaseEditorController testCaseEditor,
        IDialogService dialogService,
        TestCaseEditorController editorController,
        IUserPreferencesManager userPreferencesManager,
        ITestRunManager testRunManager,
        AppNavigationManager appNavigationManager) : base(authenticationStateProvider)
    {
        _dialogService = dialogService;
        _editorController = editorController;
        _userPreferencesManager = userPreferencesManager;
        _testRunManager = testRunManager;
        _appNavigationManager = appNavigationManager;
    }


    public async Task<TestCaseRunGridState> SearchTestCaseRunsAsync(SearchTestCaseRunQuery query, TestCaseRun? SelectedTestCaseRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var result = await _testRunManager.SearchTestCaseRunsAsync(principal, query);
        _state = new TestCaseRunGridState(query, result);

        SelectedTestCaseRun ??= _state.Data.Items.FirstOrDefault();
        if (SelectedTestCaseRun is not null)
        {
            _appNavigationManager.State.SetSelectedTestCaseRun(SelectedTestCaseRun);
        }

        return _state;
    }

    /// <summary>
    /// Runs a test case. If _state is set, we can run the next case after
    /// </summary>
    /// <param name="testCaseRun"></param>
    /// <returns></returns>
    public async Task RunTestAsync(TestCaseRun testCaseRun)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        bool continueTesting = true;
        while (continueTesting)
        {
            var parameters = new DialogParameters<RunTestCaseDialog>()
            {
                { x => x.TestCaseRun, testCaseRun }
            };
            var dialog = await _dialogService.ShowAsync<RunTestCaseDialog>(null, parameters, DefaultBehaviors.DialogOptions);
            var dialogResult = await dialog.Result;

            if (testCaseRun.Result != TestResult.NoRun && _state is not null && dialogResult is not null && dialogResult.Data is TestCaseRun)
            {
                var userPreferences = await _userPreferencesManager.LoadUserPreferencesAsync(principal);
                if (userPreferences.AdvanceToNextNotCompletedTestWhenSettingResult)
                {
                    if (_state is not null)
                    {
                        var next = await AdvanceToNextNotCompletedAsync(testCaseRun);
                        if (next is not null)
                        {
                            testCaseRun = next;
                            continueTesting = true;
                        }
                        else
                        {
                            continueTesting = false;
                        }
                    }
                }
                else
                {
                    continueTesting = false;
                }
            }
            else
            {
                continueTesting = false;
            }
        }
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
                TestState completedState = await _editorController.GetTestCaseRunFinalStateAsync(testCaseRun.TestProjectId.Value);
                testCaseRun.MappedState = completedState.MappedState;
                testCaseRun.State = completedState.Name;
            }
            else
            {
                TestState initialState = await _editorController.GetTestCaseRunInitialStateAsync(testCaseRun.TestProjectId.Value);
                testCaseRun.MappedState = initialState.MappedState;
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
    }

    private async Task<TestCaseRun?> AdvanceToNextNotCompletedAsync(TestCaseRun testCaseRun)
    {
        bool wasStateChanged = false;
        TestCaseRunGridState? initialState = _state;
        if (_state is not null)
        {
            int offset = _state.Query.Offset;
            var current = _state.Data.Items.Index().FirstOrDefault(x => x.Item.Id == testCaseRun.Id);
            if(current != default)
            {
                var index = current.Index;
                while(_state.Data.Items.Length > 0)
                {
                    index++;
                    if(_state.Data.Items.Length <= index)
                    {
                        // Load next page
                        offset += _state.Query.Count;
                        if(offset > _state.Data.TotalCount)
                        {
                            if (!wasStateChanged)
                            {
                                _state = initialState;
                            }
                            return null;
                        }
                        index = 0;
                        _state.Query.Offset = offset;
                        _state = await SearchTestCaseRunsAsync(_state.Query, null);
                        wasStateChanged = true;
                    }
                    if(_state.Data.Items.Length > index)
                    {
                        if(_state.Data.Items[index].Result == TestResult.NoRun)
                        {
                            if(wasStateChanged)
                            {
                                TestCaseRunGridStateChanged?.Invoke(this, _state);
                            }

                            return _state.Data.Items[index];
                        }
                    }
                }
            }
        }

        if (!wasStateChanged)
        {
            _state = initialState;
        }

        return null;
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
