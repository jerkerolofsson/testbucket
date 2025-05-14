using Microsoft.AspNetCore.Mvc;

using TestBucket.Components.Shared;
using TestBucket.Components.Shared.Fields;
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.TestRuns.Controls;

public partial class TestCaseRunGrid
{
    [Inject] FieldController FieldController { get; set; } = default!;

    [Parameter] public TestRun? Run { get; set; }
    [Parameter] public TestCaseRun? SelectedTestCaseRun { get; set; }
    [Parameter] public EventCallback<TestCaseRun> SelectedTestCaseRunChanged { get; set; }

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
    /// Test results based on the current filter
    /// </summary>
    private TestExecutionResultSummary? _results;

    /// <summary>
    /// Defines the filter
    /// </summary>
    private SearchTestCaseRunQuery _query = new SearchTestCaseRunQuery();

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

    public async Task OnTestCaseRunCreatedAsync(TestCaseRun testCaseRun)
    {
        if (testCaseRun.TestRunId == Run?.Id)
        {
            await ReloadResultsAsync();
        }
    }
    public async Task OnTestCaseRunUpdatedAsync(TestCaseRun testCaseRun)
    {
        if (testCaseRun.TestRunId == Run?.Id)
        {
            _dataGrid?.ReloadServerData();
            await ReloadResultsAsync();
        }
    }

    #region Lifecycle
    protected override void OnInitialized()
    {
        _selectedItem = SelectedTestCaseRun;
        testRunManager.AddObserver(this);
    }

    private TestRun? _run;

    private IReadOnlyList<FieldDefinition> _fieldDefinitions = [];
    private string _searchText = "";

    protected override async Task OnParametersSetAsync()
    {
        if (_selectedItem?.Id != SelectedTestCaseRun?.Id ||
            _selectedItem?.Result != SelectedTestCaseRun?.Result ||
            SelectedTestCaseRun is null || _run != Run)
        {
            _run = Run;
            if (_fieldDefinitions.Count == 0 && _run?.TestProjectId is not null)
            {
                _fieldDefinitions = await FieldController.GetDefinitionsAsync(_run.TestProjectId.Value, FieldTarget.TestCaseRun);
            }

            _query = SearchTestCaseRunQuery.FromUrl(_fieldDefinitions, navigationManager.Uri);
            _query.TestRunId = null;
            _searchText = _query.ToSearchText();
            _query.TestRunId = _run?.Id;

            _selectedItem = SelectedTestCaseRun;
            if (!_items.Contains(_selectedItem))
            {
                _dataGrid?.ReloadServerData();
            }
        }
    }

    public void Dispose()
    {
        testRunManager.RemoveObserver(this);
    }
    #endregion

    private async Task OnDrop(TestEntity? testEntity)
    {
        if (testEntity is null)
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
    private async Task ReloadResultsAsync()
    {
        if (Run is not null)
        {
            _results = await testBrowser.GetTestExecutionResultSummaryAsync(_query);
        }
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

    private void OnSearch(string text)
    {
        _searchText = text;
        if (_run is not null)
        {
            _query = SearchTestCaseRunQueryParser.Parse(text, _fieldDefinitions);
            _query.TestRunId = _run.Id;
            //_query.Text = text;
            _dataGrid?.ReloadServerData();
        }
    }

    private async Task ShowFilterAsync()
    {
        await Task.Yield();
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
        if (Run is null)
        {
            return new()
            {
                Items = [],
                TotalItems = 0
            };
        }
        _query.Offset = state.Page * state.PageSize;
        _query.Count = state.PageSize;

        await ReloadResultsAsync();

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

    private void FilterFailed()
    {
        _query = new SearchTestCaseRunQuery() { Result = TestResult.Failed, TestRunId = Run?.Id };
        _dataGrid?.ReloadServerData();
    }
    private void FilterIncomplete()
    {
        _query = new SearchTestCaseRunQuery() { Completed = false, TestRunId = Run?.Id };
        _dataGrid?.ReloadServerData();
    }

    private async Task FilterAssignedToMe()
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        _query = new SearchTestCaseRunQuery() { AssignedToUser = authState.User?.Identity?.Name, TestRunId = Run?.Id };
        _dataGrid?.ReloadServerData();
    }

    private void FilterUnassigned()
    {
        _query = new SearchTestCaseRunQuery() { Unassigned = true, TestRunId = Run?.Id };
        _dataGrid?.ReloadServerData();
    }

    private async Task RunTestAgain()
    {
        if (_selectedItem is null)
        {
            return;
        }
        await testExecutionController.SetTestCaseRunResultAsync(_selectedItem, TestResult.NoRun);
        //await SelectedTestCaseRunChanged.InvokeAsync(_selectedItem);
    }

}
