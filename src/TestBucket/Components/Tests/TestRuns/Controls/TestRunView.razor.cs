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
    private string _preconditions = "";
    private string _postconditions = "";
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
        _selectedTestCaseRun = testCaseRun;
        _comments = testCaseRun?.Comments ?? [];

        if (_selectedTestCaseRun?.TestCase is not null)
        {
            TestCase testCase = _selectedTestCaseRun.TestCase!;
            List<CompilerError> errors = [];

            // Generate description
            var options = new CompilationOptions(testCase, testCase.Description ?? "")
            {
                TestRunId = _selectedTestCaseRun.TestRunId,
                AllocateResources = false,
            };
            var context = await testCaseEditorController.CompileAsync(options, errors);
            _markdown = context?.CompiledText ?? "";

            // Generate pre-conditions
            options = new CompilationOptions(testCase, testCase.Preconditions ?? "")
            {
                TestRunId = _selectedTestCaseRun.TestRunId,
                AllocateResources = false,
            };
            context = await testCaseEditorController.CompileAsync(options, errors);
            _preconditions = context?.CompiledText ?? "";

            // Generate post-conditions
            options = new CompilationOptions(testCase, testCase.Postconditions ?? "")
            {
                TestRunId = _selectedTestCaseRun.TestRunId,
                AllocateResources = false,
            };
            context = await testCaseEditorController.CompileAsync(options, errors);
            _postconditions = context?.CompiledText ?? "";
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
