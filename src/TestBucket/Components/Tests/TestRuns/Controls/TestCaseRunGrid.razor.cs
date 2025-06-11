using TestBucket.Contracts.Fields;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.TestRuns.Controls;

public partial class TestCaseRunGrid
{
    [CascadingParameter] public TestProject Project { get; set; } = null!;
    [Parameter] public SearchTestCaseRunQuery? Query { get; set; }
    [Parameter] public EventCallback<SearchTestCaseRunQuery> QueryChanged { get; set; }
    [Parameter] public TestRun? Run { get; set; }
    [Parameter] public TestCaseRun? SelectedTestCaseRun { get; set; }
    [Parameter] public EventCallback<TestCaseRun> SelectedTestCaseRunChanged { get; set; }

    [Parameter] public bool CanAssign { get; set; } = false;
    [Parameter] public bool CanRun { get; set; } = false;
    [Parameter] public bool CanChangeResult { get; set; } = false;

    /// <summary>
    /// Currently displayed items
    /// </summary>
    private TestCaseRun[] _items = [];

    /// <summary>
    /// The selected item in the data grid
    /// </summary>
    private TestCaseRun? _selectedItem;

    /// <summary>
    /// Data grid
    /// </summary>
    private MudDataGrid<TestCaseRun?> _dataGrid = default!;

    /// <summary>
    /// Defines the filter
    /// </summary>
    private SearchTestCaseRunQuery _query = new SearchTestCaseRunQuery();
    private TestRun? _run;

    private IReadOnlyList<FieldDefinition> _fieldDefinitions = [];
    private string _searchText = "";
    private long _totalCount = 0;

    public Task OnRunCreatedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }
    public Task OnRunDeletedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }

    public Task OnRunUpdatedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }

    public Task OnTestCaseRunCreatedAsync(TestCaseRun testCaseRun)
    {
        if (testCaseRun.TestRunId == Run?.Id)
        {
        }
        return Task.CompletedTask;
    }
    public Task OnTestCaseRunUpdatedAsync(TestCaseRun testCaseRun)
    {
        if (testCaseRun.TestRunId == Run?.Id)
        {
            _dataGrid?.ReloadServerData();
            
        }
        return Task.CompletedTask;
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        _selectedItem = SelectedTestCaseRun;
        testRunManager.AddObserver(this);
    }
    private string? _prevSearchText;

    protected override void OnParametersSet()
    {
        _query = Query ?? new();
        _query.TestRunId = null;
        _query.ProjectId = null;
        _searchText = _query.ToSearchText();
        bool reloadGridData = _searchText != _prevSearchText;

        if (_selectedItem?.Id != SelectedTestCaseRun?.Id ||
            _selectedItem?.Result != SelectedTestCaseRun?.Result ||
            SelectedTestCaseRun is null || _run?.Id != Run?.Id)
        {
            _run = Run;
            _query.TestRunId = _run?.Id;
            _query.ProjectId = Project.Id;

            _selectedItem = SelectedTestCaseRun;
            if (_selectedItem is not null && !_items.Contains(_selectedItem))
            {
                reloadGridData = true;
            }
        }

        if(reloadGridData)
        {
            _dataGrid?.ReloadServerData();
        }

        _prevSearchText = _searchText;
    }

    public void Dispose()
    {
        testRunManager.RemoveObserver(this);
    }
    #endregion

    private async Task OnDrop(TestEntity? testEntity)
    {
        if (testEntity is null || Run is null)
        {
            return;
        }

        if (testEntity is TestCase testCase)
        {
            await OnDropTestCaseAsync(testCase);
        }
        if (testEntity is TestSuiteFolder testSuiteFolder)
        {
            await OnDropTestSuiteFolderAsync(testSuiteFolder);
        }
    }

    private async Task OnDropTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        if (Run is null || Run.TestProjectId is null)
        {
            return;
        }

        long[] testCaseIds = await testBrowser.GetTestSuiteSuiteFolderTestsAsync(folder, true);
        foreach (var testCaseId in testCaseIds)
        {
            await testRunCreationService.AddTestCaseToRunAsync(Run, testCaseId);
        }

        _dataGrid?.ReloadServerData();
    }
    private async Task OnDropTestCaseAsync(TestCase testCase)
    {
        if (Run is null || Run.TestProjectId is null)
        {
            return;
        }

        await testRunCreationService.AddTestCaseToRunAsync(Run, testCase);

        _dataGrid?.ReloadServerData();
    }

    private async Task OnClicked(TestCaseRun testCaseRun)
    {
        appNavigationManager.State.SelectedTestCaseRun = testCaseRun;
        if (_selectedItem?.Id != testCaseRun.Id)
        {
            _selectedItem = testCaseRun;
            await SelectedTestCaseRunChanged.InvokeAsync(testCaseRun);
        }
    }

    private string RowClassFunc(TestCaseRun testCaseRun, int _)
    {
        if (testCaseRun.Id == _selectedItem?.Id)
        {
            return "tb-datarow tb-datarow-selected cursor-pointer";
        }
        return "tb-datarow cursor-pointer";
    }

    private async Task SetTestCaseRunResultAsync(TestCaseRun testCaseRun, TestResult result)
    {
        await testExecutionController.SetTestCaseRunResultAsync(testCaseRun, result);
    }

    private async Task AssignRunToUserAsync(TestCaseRun testCaseRun, string user)
    {
        testCaseRun.AssignedToUserName = user;
        await testCaseEditor.SaveTestCaseRunAsync(testCaseRun);
    }

    private async Task OnSearch(string text)
    {
        _searchText = text;
        var projectId = _run?.TestProjectId ?? Project.Id;

        if(_fieldDefinitions.Count == 0)
        {
            _fieldDefinitions = await fieldController.GetDefinitionsAsync(projectId, FieldTarget.TestCaseRun);
        }
        _query = SearchTestCaseRunQueryParser.Parse(text, _fieldDefinitions);
        _query.TestRunId = null;
        _query.ProjectId = null;
        await QueryChanged.InvokeAsync(_query);
        //_dataGrid?.ReloadServerData();
    }


    public void ReloadServerData()
    {
        _dataGrid?.ReloadServerData();
    }

    public async Task SelectNextTestCaseRun()
    {
        if (_items.Length > 0)
        {
            if (_selectedItem is null)
            {
                if (_selectedItem?.Id != _items[0].Id)
                {
                    _selectedItem = _items[0];
                    await SelectedTestCaseRunChanged.InvokeAsync(_selectedItem);
                    return;
                }
            }

            var index = Array.IndexOf(_items, _selectedItem);
            index++;
            if (index < _items.Length)
            {
                if (_selectedItem?.Id != _items[index].Id)
                {
                    _selectedItem = _items[index];
                    await SelectedTestCaseRunChanged.InvokeAsync(_selectedItem);
                }
            }
            else
            {
                // Next page?
            }
        }
    }

    private async Task<GridData<TestCaseRun>> LoadGridData(GridState<TestCaseRun> state)
    {
        //if (Run is null)
        //{
        //    return new()
        //    {
        //        Items = [],
        //        TotalItems = 0
        //    };
        //}
        _query.Offset = state.Page * state.PageSize;
        _query.Count = state.PageSize;
        _query.TestRunId = _run?.Id;
        _query.ProjectId = Project.Id;

        var searchTestCaseRunsResult = await testBrowser.SearchTestCaseRunsAsync(_query);
        _items = searchTestCaseRunsResult.Items;

        // If we reloaded and there was a selected item. Update it.
        if(_selectedItem is not null)
        {
            _selectedItem = _items.FirstOrDefault(x => x.Id == _selectedItem.Id);
            await SelectedTestCaseRunChanged.InvokeAsync(_selectedItem);
        }

        if (_selectedItem is null)
        {
            await SelectFirstItemIfNoSelectionOrCurrentSelectionIsNotFoundAsync();
        }

        if (_totalCount != searchTestCaseRunsResult.TotalCount)
        {
            _totalCount = searchTestCaseRunsResult.TotalCount;
            await InvokeAsync(StateHasChanged);
        }
        GridData<TestCaseRun> data = new()
        {
            Items = _items,
            TotalItems = (int)searchTestCaseRunsResult.TotalCount
        };

        return data;
    }

    private async Task SelectFirstItemIfNoSelectionOrCurrentSelectionIsNotFoundAsync()
    {
        if (_items.Length > 0)
        {
            if (_selectedItem is null || !_items.Contains(_selectedItem))
            {
                _selectedItem = _items[0];
                await SelectedTestCaseRunChanged.InvokeAsync(_selectedItem);
            }
        }
    }

    private async Task RunTestAgain(TestCaseRun testCaseRun)
    {
        if (_selectedItem is null)
        {
            return;
        }
        await testExecutionController.SetTestCaseRunResultAsync(testCaseRun, TestResult.NoRun);

        await testExecutionController.RunTestAsync(testCaseRun);
        //await SelectedTestCaseRunChanged.InvokeAsync(_selectedItem);
    }

    private async Task RunTest(TestCaseRun testCaseRun)
    {
        if (_selectedItem is null)
        {
            return;
        }
        await testExecutionController.RunTestAsync(testCaseRun);
    }

    private void SetSelectedTestCaseRun(TestCaseRun testCaseRun)
    {
        appNavigationManager.State.SelectedTestCaseRun = testCaseRun;
    }
}
