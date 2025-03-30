using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Components.Tests.Controls;

public partial class TestRunView
{
    [Parameter] public TestRun? TestRun { get; set; }

    //private TestCase? _selectedTestCase = null;
    private TestCaseRun? _selectedTestCaseRun = null;
    private TestCaseRunGrid? testCaseRunGrid;
    private string _markdown = "";

    protected override void OnParametersSet()
    {
        _selectedTestCaseRun = null;


        base.OnParametersSet();
    }

    private async Task OnSelectedTestCaseRunChanged(TestCaseRun testCaseRun)
    {
        _selectedTestCaseRun = testCaseRun;
        if (_selectedTestCaseRun?.TestCase is not null)
        {
            TestCase testCase = _selectedTestCaseRun.TestCase!;
            string description = testCase.Description ?? "";
            long testRunId = _selectedTestCaseRun.TestRunId;
            _markdown = (await testCaseEditorController.CompileTestCaseRunPreviewAsync(testCase, testRunId, description)) ?? "";
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
