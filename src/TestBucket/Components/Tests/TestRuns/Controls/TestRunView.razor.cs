using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Comments.Models;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.TestRuns.Controls;

public partial class TestRunView
{
    [Parameter] public TestRun? TestRun { get; set; }
    [Parameter] public SearchTestCaseRunQuery? Query { get; set; }
    [Parameter] public EventCallback<SearchTestCaseRunQuery> QueryChanged { get; set; }

    private TestCaseRun? _selectedTestCaseRun = null;
    private TestCaseRunGrid? testCaseRunGrid;
    private string _markdown = "";
    private List<Comment> _comments = [];
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

    private async Task OnQueryChanged(SearchTestCaseRunQuery query)
    {
        await QueryChanged.InvokeAsync(query);
    }

    private int _activePanelIndex = 0;
    private void OnActivePanelIndexChanged(int index)
    {
        _activePanelIndex = index;
    }

    private async Task OnCommentAdded(Comment comment)
    {
        if (_selectedTestCaseRun is not null)
        {
            comment.TeamId = _selectedTestCaseRun.TeamId;
            comment.TestProjectId = _selectedTestCaseRun.TestProjectId;
            comment.TestCaseRunId = _selectedTestCaseRun.Id;
            _comments.Add(comment);
            await comments.AddCommentAsync(comment);
        }
    }
    private async Task OnCommentDeleted(Comment comment)
    {
        _comments.Remove(comment);
        await comments.DeleteCommentAsync(comment);
    }

    protected override void OnParametersSet()
    {
        Query ??= new();
        Query.TestRunId = TestRun?.Id;
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
        _comments = testCaseRun?.Comments ?? [];

        if (_selectedTestCaseRun?.TestCase is not null)
        {
            TestCase testCase = _selectedTestCaseRun.TestCase!;
            string description = testCase.Description ?? "";
            long testRunId = _selectedTestCaseRun.TestRunId;
            List<CompilerError> errors = new List<CompilerError>();

            var options = new CompilationOptions(testCase, description)
            {
                TestRunId = testRunId,
                AllocateResources = false,
            };
            _markdown = await testCaseEditorController.CompileAsync(options, errors) ?? "";
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

    public ValueTask DisposeAsync()
    {
        testExecutionController.TestCompleted -= OnTestCompleted;
        return ValueTask.CompletedTask;
    }
}
