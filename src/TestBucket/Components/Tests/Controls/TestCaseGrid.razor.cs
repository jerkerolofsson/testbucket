namespace TestBucket.Components.Tests.Controls;

public partial class TestCaseGrid
{
    [Parameter] public bool CompareFolder { get; set; } = true;
    [Parameter] public TestSuiteFolder? Folder { get; set; }
    [Parameter] public long? FolderId { get; set; }
    [Parameter] public long? TestSuiteId { get; set; }
    [Parameter] public EventCallback<TestCase> OnTestCaseClicked { get; set; }

    private TestSuiteFolder? _folder;

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

    protected override async Task OnParametersSetAsync()
    {
        if (FolderId is not null)
        {
            if(FolderId != _folder?.Id)
            {
                _folder = await testSuiteServer.GetTestSuiteFolderByIdAsync(FolderId.Value);
                _dataGrid?.ReloadServerData();
            }
        }
        else if(Folder is not null)
        {
            if(_folder?.Id != Folder.Id)
            {
                _folder = Folder;
                _dataGrid?.ReloadServerData();
            }
        }
    }
    protected override void OnInitialized()
    {
        testCaseEditor.AddObserver(this);
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
        if (TestSuiteId is not null || _folder is not null)
        {
            TestCase? testCase = await testCaseEditor.CreateNewTestCaseAsync(_folder, TestSuiteId);
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
        var query = new SearchTestQuery
        {
            CompareFolder = CompareFolder,
            TestSuiteId = TestSuiteId,
            FolderId = _folder?.Id,
            Text = string.IsNullOrWhiteSpace(_searchText) ? null : _searchText,
        };

        var result = await testBrowser.SearchTestCasesAsync(query, state.Page * state.PageSize, state.PageSize);

        GridData<TestCase> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        return data;
    }

}
