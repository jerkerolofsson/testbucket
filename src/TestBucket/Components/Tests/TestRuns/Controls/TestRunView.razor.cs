using System.Diagnostics;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Components.Tests.TestRuns.Controls;

public partial class TestRunView
{
    [Parameter] public TestRun? TestRun { get; set; }

    private TestCaseRun? _selectedTestCaseRun = null;
    private TestCaseRunGrid? testCaseRunGrid;
    private string _markdown = "";

    private TestState? State 
    {
        get
        {
            if(_selectedTestCaseRun?.State is not null)
            {
                return new TestState() { MappedState = _selectedTestCaseRun.MappedState ?? MappedTestState.NotStarted, Name = _selectedTestCaseRun.State };
            }
            return null;
        }
    }

    private int _activePanelIndex = 0;
    private void OnActivePanelIndexChanged(int index)
    {
        _activePanelIndex = index;
    }

    private void OnSelectedTestCaseRunChanged()
    {
        if(_selectedTestCaseRun  is null)
        {
            
        }
    }

    protected override void OnParametersSet()
    {
        _selectedTestCaseRun = null;
        base.OnParametersSet();
    }

    private async Task OnSelectedTestCaseRunChanged(TestCaseRun? testCaseRun)
    {
        // Change the tab if changing test case, but keep the default if just opening
        // the view for the first time
        if (_selectedTestCaseRun is not null && _selectedTestCaseRun?.Id != testCaseRun?.Id)
        {
            _activePanelIndex = 1;
        }

        _selectedTestCaseRun = testCaseRun;

        if (_selectedTestCaseRun?.TestCase is not null)
        {
            TestCase testCase = _selectedTestCaseRun.TestCase!;
            string description = testCase.Description ?? "";
            long testRunId = _selectedTestCaseRun.TestRunId;
            List<CompilerError> errors = new List<CompilerError>();
            _markdown = await testCaseEditorController.CompileTestCaseRunPreviewAsync(testCase, testRunId, description, errors) ?? "";
        }
    }

    private async Task OnTestCaseRunChanged(TestCaseRun run)
    {
        if (_selectedTestCaseRun?.Id == run?.Id)
        {
            await testCaseEditorController.ReleaseResourcesAsync();
            _selectedTestCaseRun = run;
            if (testCaseRunGrid is not null)
            {
                testCaseRunGrid.ReloadServerData();
            }
        }
    }

    private async Task OnTestCaseRunStateChanged(TestState? state)
    {
        if (_selectedTestCaseRun?.TestCase is not null)
        {
            _selectedTestCaseRun.State = state?.Name;
            _selectedTestCaseRun.MappedState = state?.MappedState;

            await testCaseEditorController.SaveTestCaseRunAsync(_selectedTestCaseRun);
        }
    }

    private async Task OnTestCaseRunResultChanged(TestResult result)
    {
        if (_selectedTestCaseRun?.TestProjectId is not null)
        {
            await testExecutionController.SetTestCaseRunResultAsync(_selectedTestCaseRun, result);
        }
    }


    private void OnTestCompleted(object? sender, TestCaseRun testCaseRun)
    {
        if (_selectedTestCaseRun == testCaseRun && testCaseRunGrid is not null)
        {
            var _ = InvokeAsync(async () => 
            {
                await testCaseRunGrid.SelectNextTestCaseRun();
            });
        }
    }
    protected override void OnInitialized()
    {
        testExecutionController.TestCompleted += OnTestCompleted;
    }

    public async ValueTask DisposeAsync()
    {
        testExecutionController.TestCompleted -= OnTestCompleted;
        await testCaseEditorController.ReleaseResourcesAsync();
    }
}
