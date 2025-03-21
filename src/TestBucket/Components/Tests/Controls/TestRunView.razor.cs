using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Components.Tests.Controls;

public partial class TestRunView
{
    [Parameter] public TestRun? TestRun { get; set; }

    //private TestCase? _selectedTestCase = null;
    private TestCaseRun? _selectedTestCaseRun = null;

    private async Task OnTestCaseRunStateChanged(string? state)
    {
        if (_selectedTestCaseRun is not null)
        {
            _selectedTestCaseRun.State = state;
            await testCaseEditorService.SaveTestCaseRunAsync(_selectedTestCaseRun);
        }
    }

    private async Task OnTestCaseRunResultChanged(TestResult result)
    {
        if (_selectedTestCaseRun?.TestProjectId is not null)
        {
            await testExecutionController.SetTestCaseRunResultAsync(_selectedTestCaseRun, result);
        }
    }

    private Task OnTestCaseRunClicked(TestCaseRun testCaseRun)
    {
        _selectedTestCaseRun = testCaseRun;
        return Task.CompletedTask;
    }

}
