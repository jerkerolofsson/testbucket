
using TestBucket.Components.Tests.Models;
using TestBucket.Components.Tests.TestCases.Dialogs;

namespace TestBucket.Components.Tests.TestCases.Controls;

public partial class TestCaseGrid
{
    [Parameter] public bool CompareFolder { get; set; } = true;
    [Parameter] public TestSuiteFolder? Folder { get; set; }
    [Parameter] public long? FolderId { get; set; }
    [Parameter] public TestProject? Project { get; set; }
    [Parameter] public long? TestSuiteId { get; set; }
    [Parameter] public EventCallback<TestCase> OnTestCaseClicked { get; set; }
    [Parameter] public EventCallback<TestSuiteFolder> OnTestSuiteFolderClicked { get; set; }

    [Parameter] public SearchTestQuery? Query { get; set; }
    [Parameter] public EventCallback<SearchTestQuery> QueryChanged { get; set; }

    private TestSuiteFolder? _folder;

    private SearchTestQuery _query = new();

    private MudDataGrid<TestSuiteItem?> _dataGrid = default!;

    private List<FieldDefinition> _columns = [];
    private long? _projectId;

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

    private long? _testSuiteId = null;
    private long? _folderId = null;
    private bool _hasCustomFilter = false;

    protected override async Task OnParametersSetAsync()
    {
        bool changed = false;

        _query.CompareFolder = CompareFolder;
        _query.TestSuiteId = TestSuiteId;
        if(_testSuiteId != TestSuiteId)
        {
            _testSuiteId = TestSuiteId;
            changed = true;
        }
        if (_folderId != FolderId)
        {
            _folderId = FolderId;
            _folder = null;
            changed = true;
            if (_folderId is not null)
            {
                _folder = await testSuiteServer.GetTestSuiteFolderByIdAsync(_folderId.Value);
            }
        }
        else if(Folder is not null)
        {
            if(_folder?.Id != Folder.Id)
            {
                changed = true;
                _folder = Folder;
            }
        }

        if(Project is not null && _projectId != Project.Id)
        {
            changed = true;
            _projectId = Project.Id;
        }

        if (changed)
        {
            _dataGrid?.ReloadServerData();
        }

        _columns = [];
    }
    protected override void OnInitialized()
    {
        testCaseManager.AddObserver(this);
    }
    public void Dispose()
    {
        testCaseManager.RemoveObserver(this);
    }
    #endregion
    private string RowClassFunc(TestSuiteItem item, int _)
    {
        //if (testCaseRun.Id == appNa?.Id)
        //{
        //    return "tb-datarow tb-datarow-selected cursor-pointer";
        //}
        return "tb-datarow cursor-pointer";
    }
    private async Task OnItemClicked(TestSuiteItem item)
    {
        if (item.TestCase is not null)
        {
            await OnTestClicked(item.TestCase);
        }
        if (item.Folder is not null)
        {
            await OnFolderClicked(item.Folder);
        }
    }

    private async Task OnFolderClicked(TestSuiteFolder folder)
    {
        await OnTestSuiteFolderClicked.InvokeAsync(folder);
    }
    private async Task OnTestClicked(TestCase testCase)
    {
        await OnTestCaseClicked.InvokeAsync(testCase);
    }

    private async Task ResetFilter()
    {
        _hasCustomFilter = false;
        await QueryChanged.InvokeAsync(null);
        _dataGrid?.ReloadServerData();
    }

    private async Task ShowFilterAsync()
    {
        var parameters = new DialogParameters<EditTestCaseFilterDialog>
        {
            { x => x.Query, _query },
            { x => x.Project, Project },
        };
        var dialog = await dialogService.ShowAsync<EditTestCaseFilterDialog>(loc["filter-tests"], parameters);
        var result = await dialog.Result;
        if (result?.Data is SearchTestQuery query)
        {
            _hasCustomFilter = true;
            _query = query;
            _query.CompareFolder = false;
            await QueryChanged.InvokeAsync(_query);
            _dataGrid?.ReloadServerData();
        }
    }

    private async Task CreateNewTestCaseAsync()
    {
        if(Project is null)
        {
            return;
        }

        if (TestSuiteId is not null || _folder is not null)
        {
            TestCase? testCase = await testCaseEditor.CreateNewTestCaseAsync(Project, _folder, TestSuiteId);
            if (testCase is not null)
            {
                _dataGrid?.ReloadServerData();
            }
        }
    }

    private void OnSearch(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            _query.CompareFolder = true;
            _query.Text = null;
            _hasCustomFilter = false;
        }
        else
        {
            _query.CompareFolder = false; // When searching for text, disable the compare folder so we search all 
            _query.Text = text;
            _hasCustomFilter = true;

        }
        _dataGrid?.ReloadServerData();
    }

    private async Task<GridData<TestSuiteItem>> LoadGridData(GridState<TestSuiteItem> state)
    {
        var query = new SearchTestQuery
        {
            CompareFolder = _query.CompareFolder,
            TestSuiteId = _query.TestSuiteId,
            FolderId = _folder?.Id,
            CreatedFrom = _query.CreatedFrom,
            CreatedUntil = _query.CreatedUntil,
            ProjectId = Project?.Id,
            Fields = _query.Fields,

            Text = string.IsNullOrWhiteSpace(_query.Text) ? null : _query.Text,
        };

        var result = await testBrowser.SearchItemsAsync(query, state.Page * state.PageSize, state.PageSize, !_hasCustomFilter);

        GridData<TestSuiteItem> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        return data;
    }

}
