namespace TestBucket.Components.Tests.Controls;

public partial class TestCaseGrid
{
    [Parameter] public TestSuiteFolder? Folder { get; set; }
    [Parameter] public long? TestSuiteId { get; set; }
    [Parameter] public EventCallback<TestCase> OnTestCaseClicked { get; set; }

    private string? _searchText;

    private MudDataGrid<TestCase?> _dataGrid = default!;

    public Task OnTestCreatedAsync(TestCase testCase)
    {
        _dataGrid?.ReloadServerData();
        return Task.CompletedTask;
    }
    public Task OnTestDeletedAsync(TestCase testCase)
    {
        _dataGrid?.ReloadServerData();
        return Task.CompletedTask;
    }
    public Task OnTestSavedAsync(TestCase testCase)
    {
        _dataGrid?.ReloadServerData();
        return Task.CompletedTask;
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        testCaseEditor.AddObserver(this);
    }

    protected override void OnParametersSet()
    {
        _dataGrid?.ReloadServerData();
    }

    public void Dispose()
    {
        testCaseEditor.RemoveObserver(this);
    }
    #endregion

    private async Task OnTestClicked(TestCase testCase)
    {
        await OnTestCaseClicked.InvokeAsync(testCase);
    }

    private async Task CreateNewTestCaseAsync()
    {
        if (TestSuiteId is not null || Folder is not null)
        {
            TestCase? testCase = await testCaseEditor.CreateNewTestCaseAsync(Folder, TestSuiteId);
            if (testCase is not null)
            {
                _dataGrid?.ReloadServerData();
            }
        }
    }

    private void OnSearch(string text)
    {
        _searchText = text;
        _dataGrid?.ReloadServerData();
    }

    private async Task<GridData<TestCase>> LoadGridData(GridState<TestCase> state)
    {
        var result = await testBrowser.SearchTestCasesAsync(TestSuiteId, Folder?.Id, _searchText, state.Page * state.PageSize, state.PageSize);

        GridData<TestCase> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        return data;
    }

}
