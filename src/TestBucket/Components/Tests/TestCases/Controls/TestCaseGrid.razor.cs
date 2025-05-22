
using TestBucket.Components.Tests.Models;
using TestBucket.Components.Tests.TestCases.Dialogs;
using TestBucket.Domain.Testing.TestCases.Search;

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

    [Parameter] public EventCallback<long> TotalNumberOfTestsChanged { get; set; }

    private TestSuiteFolder? _folder;

    private SearchTestQuery _query = new();

    private MudDataGrid<TestSuiteItem?> _dataGrid = default!;

    private long? _projectId;

    private bool _hasQueryChanged = false;

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

    private long? _folderId = null;
    private bool _hasCustomFilter = false;
    private IReadOnlyList<FieldDefinition> _fields = [];

    protected override async Task OnParametersSetAsync()
    {
        if(Query is null)
        {
            return;
        }
        _hasQueryChanged = !_query.Equals(Query);
        if (_hasQueryChanged)
        {
            _query = Query;
            _searchPhrase = _query.ToSearchText();
        }

        if (_folderId != FolderId)
        {
            _folderId = FolderId;
            _folder = null;
            if (_folderId is not null)
            {
                _folder = await testSuiteServer.GetTestSuiteFolderByIdAsync(_folderId.Value);
            }
        }
        else if(Folder is not null)
        {
            if(_folder?.Id != Folder.Id)
            {
                _folder = Folder;
            }
        }

        if(Project is not null && _projectId != Project.Id)
        {
            _projectId = Project.Id;
            _fields = await fieldController.GetDefinitionsAsync(_projectId.Value, Contracts.Fields.FieldTarget.TestCase);
        }

        if (_hasQueryChanged)
        {
            _dataGrid?.ReloadServerData();
        }
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
        _hasQueryChanged = true;
        _hasCustomFilter = false;
        await QueryChanged.InvokeAsync(null);
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
            await OnSearch(_query.ToSearchText());
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

    private string _searchPhrase = "";

    private async Task OnSearch(string text)
    {
        _query = SearchTestCaseQueryParser.Parse(text, _fields);
        _searchPhrase = _query.ToSearchText();
        _query.FolderId = Query?.FolderId;
        _query.CompareFolder = Query?.CompareFolder;

        if (string.IsNullOrEmpty(text))
        {
            _hasCustomFilter = false;
        }
        else
        {
            _hasCustomFilter = true;

        }
        await QueryChanged.InvokeAsync(_query);
        _dataGrid?.ReloadServerData();
    }

    private async Task<GridData<TestSuiteItem>> LoadGridData(GridState<TestSuiteItem> state)
    {
        _query.ProjectId = Project?.Id;
        var result = await testBrowser.SearchItemsAsync(_query, state.Page * state.PageSize, state.PageSize, !_hasCustomFilter);

        GridData<TestSuiteItem> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        await TotalNumberOfTestsChanged.InvokeAsync(result.TotalCount);

        return data;
    }

}
