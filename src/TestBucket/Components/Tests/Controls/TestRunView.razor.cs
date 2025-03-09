using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Components.Tests.Controls;

public partial class TestRunView
{
    [Parameter] public TestRun? TestRun { get; set; }

    private TestCase? _selectedTestCase = null;
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
        if (_selectedTestCaseRun is not null)
        {
            _selectedTestCaseRun.Result = result;
            if (_selectedTestCaseRun.Result != TestResult.NoRun)
            {
                TestState completedState = await testCaseEditorService.GetProjectFinalStateAsync(_selectedTestCaseRun.TestProjectId);
                _selectedTestCaseRun.State = completedState.Name;
            }
            else
            {
                TestState initialState = await testCaseEditorService.GetProjectInitialStateAsync(_selectedTestCaseRun.TestProjectId);
                _selectedTestCaseRun.State = initialState.Name;
            }

            await testCaseEditorService.SaveTestCaseRunAsync(_selectedTestCaseRun);
        }
    }

    private Task OnTestCaseRunClicked(TestCaseRun testCaseRun)
    {
        _selectedTestCaseRun = testCaseRun;
        return Task.CompletedTask;
    }

}
