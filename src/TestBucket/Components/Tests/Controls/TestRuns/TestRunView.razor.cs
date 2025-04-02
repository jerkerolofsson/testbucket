using TestBucket.Components.Tests.Controls.TestCaseRuns;
using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Components.Tests.Controls.TestRuns;

public partial class TestRunView
{
    [Parameter] public TestRun? TestRun { get; set; }

    //private TestCase? _selectedTestCase = null;
    private TestCaseRun? _selectedTestCaseRun = null;
    private TestCaseRunGrid? testCaseRunGrid;
    private string _markdown = "";

    private int _activePanelIndex = 0;
    private void OnActivePanelIndexChanged(int index)
    {
        _activePanelIndex = index;
    }

    protected override void OnParametersSet()
    {
        _selectedTestCaseRun = null;
        base.OnParametersSet();
    }

    private async Task OnSelectedTestCaseRunChanged(TestCaseRun testCaseRun)
    {
        // Change the tab if changing test case, but keep the default if just opening
        // the view for the first time
        if (_selectedTestCaseRun is not null && _selectedTestCaseRun?.Id != testCaseRun.Id)
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
            _markdown = (await testCaseEditorController.CompileTestCaseRunPreviewAsync(testCase, testRunId, description, errors)) ?? "";
        }
    }

    private async Task OnTestCaseRunStateChanged(string? state)
    {
        if (_selectedTestCaseRun?.TestCase is not null)
        {
            _selectedTestCaseRun.State = state;

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

    //private Task OnTestCaseRunClicked(TestCaseRun testCaseRun)
    //{
    //    _selectedTestCaseRun = testCaseRun;
    //    return Task.CompletedTask;
    //}

    private void OnTestCompleted(object? sender, TestCaseRun testCaseRun)
    {
        if (_selectedTestCaseRun == testCaseRun && testCaseRunGrid is not null)
        {
            InvokeAsync(() => testCaseRunGrid.SelectNextTestCaseRun() );
        }
    }
    protected override void OnInitialized()
    {
        testExecutionController.TestCompleted += OnTestCompleted;
    }

    public void Dispose()
    {
        testExecutionController.TestCompleted -= OnTestCompleted;
    }
}
