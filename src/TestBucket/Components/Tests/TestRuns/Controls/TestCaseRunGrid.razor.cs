
using TestBucket.Components.Tests.TestRuns.ViewModels;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Components.Tests.TestRuns.Controls;

/// <summary>
/// Displays and manages a data grid of <see cref="TestCaseRun"/>s for a given <see cref="TestRun"/>.
/// Supports selection, assignment, result changes, and drag-and-drop operations.
/// </summary>
public partial class TestCaseRunGrid
{
    /// <summary>
    /// The current test project context, provided via cascading parameter.
    /// </summary>
    [CascadingParameter] public TestProject Project { get; set; } = null!;

    /// <summary>
    /// The current search query for filtering test case runs.
    /// </summary>
    [Parameter] public SearchTestCaseRunQuery? Query { get; set; }

    /// <summary>
    /// Event callback invoked when the search query changes.
    /// </summary>
    [Parameter] public EventCallback<SearchTestCaseRunQuery> QueryChanged { get; set; }

    /// <summary>
    /// The test run whose test case runs are displayed.
    /// </summary>
    [Parameter] public TestRun? Run { get; set; }

    /// <summary>
    /// The currently selected test case run.
    /// </summary>
    [Parameter] public TestCaseRun? SelectedTestCaseRun { get; set; }

    /// <summary>
    /// Event callback invoked when the selected test case run changes.
    /// </summary>
    [Parameter] public EventCallback<TestCaseRun> SelectedTestCaseRunChanged { get; set; }

    /// <summary>
    /// Indicates if the user can assign test case runs.
    /// </summary>
    [Parameter] public bool CanAssign { get; set; } = false;

    /// <summary>
    /// Indicates if the user can run test cases.
    /// </summary>
    [Parameter] public bool CanRun { get; set; } = false;

    /// <summary>
    /// Indicates if the user can change the result of test case runs.
    /// </summary>
    [Parameter] public bool CanChangeResult { get; set; } = false;

    /// <summary>
    /// Current items
    /// </summary>
    private TestCaseRunGridState? _state = null;

    ///// <summary>
    ///// Currently displayed items in the grid.
    ///// </summary>
    //private TestCaseRun[] _items = [];

    ///// <summary>
    ///// The selected item in the data grid.
    ///// </summary>
    private TestCaseRun? _selectedItem;

    /// <summary>
    /// Reference to the MudDataGrid component.
    /// </summary>
    private MudDataGrid<TestCaseRun?> _dataGrid = default!;

    /// <summary>
    /// The current filter query for the grid.
    /// </summary>
    private SearchTestCaseRunQuery _query = new SearchTestCaseRunQuery();

    /// <summary>
    /// The current test run being displayed.
    /// </summary>
    private TestRun? _run;

    /// <summary>
    /// Field definitions for the current project, used for search and display.
    /// </summary>
    private IReadOnlyList<FieldDefinition> _fieldDefinitions = [];

    /// <summary>
    /// The current search text.
    /// </summary>
    private string _searchText = "";

    /// <summary>
    /// The total number of items matching the current query.
    /// </summary>
    private long _totalCount = 0;

    /// <summary>
    /// The previous search text, used to detect changes.
    /// </summary>
    private string? _prevSearchText;

    /// <summary>
    /// Called when a new test run is created.
    /// </summary>
    /// <param name="testRun">The created test run.</param>
    public Task OnRunCreatedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }


    public Task OnRunMovedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a test run is deleted.
    /// </summary>
    /// <param name="testRun">The deleted test run.</param>
    public Task OnRunDeletedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a test run is updated.
    /// </summary>
    /// <param name="testRun">The updated test run.</param>
    public Task OnRunUpdatedAsync(TestRun testRun)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a new test case run is created.
    /// </summary>
    /// <param name="testCaseRun">The created test case run.</param>
    public Task OnTestCaseRunCreatedAsync(TestCaseRun testCaseRun)
    {
        // Only handle if the test case run belongs to the current run.
        if (testCaseRun.TestRunId == Run?.Id)
        {
            // No-op, placeholder for future logic.
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a test case run is updated.
    /// </summary>
    /// <param name="testCaseRun">The updated test case run.</param>
    public Task OnTestCaseRunUpdatedAsync(TestCaseRun testCaseRun)
    {
        // If the updated test case run belongs to the current run, reload the grid data.
        if (testCaseRun.TestRunId == Run?.Id)
        {
            _dataGrid?.ReloadServerData();
        }
        return Task.CompletedTask;
    }

    #region Lifecycle

    /// <summary>
    /// Initializes the component and registers as an observer.
    /// </summary>
    protected override void OnInitialized()
    {
        testExecutionController.TestCaseRunGridStateChanged += OnTestCaseRunGridStateChanged;
        _selectedItem = SelectedTestCaseRun;
        testRunManager.AddObserver(this);
    }

    private void OnTestCaseRunGridStateChanged(object? sender, TestCaseRunGridState state)
    {
        _state = state;
        if (_dataGrid is not null)
        {
            _currentPage = _state.Query.Offset / _state.Query.Count;

            //_dataGrid.CurrentPage = _state.Query.Offset / _state.Query.Count;
            _dataGrid.ReloadServerData();
        }
    }

    /// <summary>
    /// Handles parameter changes and updates the grid/filter state accordingly.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        _query = Query ?? new();
        _query.TestRunId = null;
        _query.ProjectId = null;
        _query.Count = _rowsPerPage;
        _searchText = _query.ToSearchText();
        bool reloadGridData = _searchText != _prevSearchText;

        // Update selection and run context if changed.
        if (_selectedItem?.Id != SelectedTestCaseRun?.Id ||
            _selectedItem?.Result != SelectedTestCaseRun?.Result ||
            SelectedTestCaseRun is null || _run?.Id != Run?.Id)
        {
            if(_run?.Id != Run?.Id)
            {
                reloadGridData = true;
            }

            _run = Run;
            _selectedItem = SelectedTestCaseRun;
        }
        _query.TestRunId = _run?.Id;
        _query.ProjectId = Project.Id;

        if (reloadGridData)
        {
            //_dataGrid?.ReloadServerData();
            await ReloadDataAsync();
        }

        _prevSearchText = _searchText;
    }

    /// <summary>
    /// Disposes the component and unregisters as an observer.
    /// </summary>
    public void Dispose()
    {
        testExecutionController.TestCaseRunGridStateChanged -= OnTestCaseRunGridStateChanged;
        testRunManager.RemoveObserver(this);
    }
    #endregion

    /// <summary>
    /// Handles drop events for test entities (test cases or folders).
    /// </summary>
    /// <param name="testEntity">The dropped test entity.</param>
    private async Task OnDrop(TestEntity? testEntity)
    {
        if (testEntity is null || Run is null)
        {
            return;
        }

        // Handle dropping a test case.
        if (testEntity is TestCase testCase)
        {
            await OnDropTestCaseAsync(testCase);
        }
        // Handle dropping a test suite folder.
        if (testEntity is TestSuiteFolder testSuiteFolder)
        {
            await OnDropTestSuiteFolderAsync(testSuiteFolder);
        }
    }

    /// <summary>
    /// Handles dropping a test suite folder onto the grid.
    /// Adds all test cases in the folder to the current run.
    /// </summary>
    /// <param name="folder">The dropped test suite folder.</param>
    private async Task OnDropTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        if (Run is null || Run.TestProjectId is null)
        {
            return;
        }

        // Get all test case IDs in the folder (recursively).
        long[] testCaseIds = await testBrowser.GetTestSuiteSuiteFolderTestsAsync(folder, true);
        foreach (var testCaseId in testCaseIds)
        {
            await testRunCreationService.AddTestCaseToRunAsync(Run, testCaseId);
        }

        await ReloadDataAsync();
        //_dataGrid?.ReloadServerData();
    }

    /// <summary>
    /// Handles dropping a single test case onto the grid.
    /// Adds the test case to the current run.
    /// </summary>
    /// <param name="testCase">The dropped test case.</param>
    private async Task OnDropTestCaseAsync(TestCase testCase)
    {
        if (Run is null || Run.TestProjectId is null)
        {
            return;
        }

        await testRunCreationService.AddTestCaseToRunAsync(Run, testCase);

        //_dataGrid?.ReloadServerData();
        await ReloadDataAsync();
    }

    /// <summary>
    /// Handles clicking on a test case run row.
    /// Updates the selected item and notifies parent.
    /// </summary>
    /// <param name="testCaseRun">The clicked test case run.</param>
    private async Task OnClicked(TestCaseRun testCaseRun)
    {
        appNavigationManager.State.SetSelectedTestCaseRun(testCaseRun);
        appNavigationManager.State.SetMultiSelectedTestCaseRuns([testCaseRun]);
        if (_selectedItem?.Id != testCaseRun.Id)
        {
            _selectedItem = testCaseRun;
            await SelectedTestCaseRunChanged.InvokeAsync(testCaseRun);
        }
    }

    public void SetSelected(TestCaseRun testCaseRun, bool selected)
    {
        if (appNavigationManager.State.MultiSelectedTestCaseRuns.Contains(testCaseRun))
        {
            if (!selected)
            {
                if (_selectedItem?.Id == testCaseRun.Id)
                {
                    _selectedItem = null;
                }
                var testCaseRuns = appNavigationManager.State.MultiSelectedTestCaseRuns.Where(x => x.Id != testCaseRun.Id);
                appNavigationManager.State.SetMultiSelectedTestCaseRuns(testCaseRuns.ToList());
            }
        }
        else if(selected)
        {
            _selectedItem = testCaseRun;
            appNavigationManager.State.SetMultiSelectedTestCaseRuns([testCaseRun, .. appNavigationManager.State.MultiSelectedTestCaseRuns]);
        }
    }

    public bool IsSelected(TestCaseRun testCaseRun)
    {
        return appNavigationManager.State.MultiSelectedTestCaseRuns.Any(x => x.Id == testCaseRun.Id);
    }

    /// <summary>
    /// Returns the CSS class for a row based on selection.
    /// </summary>
    /// <param name="testCaseRun">The test case run for the row.</param>
    /// <param name="_">Unused row index.</param>
    /// <returns>CSS class string.</returns>
    private string RowClassFunc(TestCaseRun testCaseRun, int _)
    {
        if (testCaseRun.Id == _selectedItem?.Id)
        {
            return "tb-datarow tb-datarow-selected cursor-pointer";
        }
        return "tb-datarow cursor-pointer";
    }

    /// <summary>
    /// Sets the result for a test case run.
    /// </summary>
    /// <param name="testCaseRun">The test case run to update.</param>
    /// <param name="result">The new result.</param>
    private async Task SetTestCaseRunResultAsync(TestCaseRun testCaseRun, TestResult result)
    {
        await testExecutionController.SetTestCaseRunResultAsync(testCaseRun, result);
        await ReloadDataAsync();
    }

    /// <summary>
    /// Assigns a test case run to a user and saves the change.
    /// </summary>
    /// <param name="testCaseRun">The test case run to assign.</param>
    /// <param name="user">The user name to assign to.</param>
    private async Task AssignRunToUserAsync(TestCaseRun testCaseRun, string user)
    {
        testCaseRun.AssignedToUserName = user;
        await testCaseEditor.SaveTestCaseRunAsync(testCaseRun);
        await ReloadDataAsync();
    }

    /// <summary>
    /// Handles search input, updates the query, and notifies parent.
    /// </summary>
    /// <param name="text">The search text.</param>
    private async Task OnSearch(string text)
    {
        _searchText = text;
        var projectId = _run?.TestProjectId ?? Project.Id;

        // Load field definitions if not already loaded.
        if (_fieldDefinitions.Count == 0)
        {
            _fieldDefinitions = await fieldController.GetDefinitionsAsync(projectId, FieldTarget.TestCaseRun);
        }
        _query = SearchTestCaseRunQueryParser.Parse(text, _fieldDefinitions);
        _query.TestRunId = null;
        _query.ProjectId = null;
        await QueryChanged.InvokeAsync(_query);
    }

    private async Task OnQueryChanged(SearchTestCaseRunQuery query)
    {
        await QueryChanged.InvokeAsync(query);
    }



    private int _rowsPerPage = 15;
    private int _currentPage = 1;

    private async Task OnRowsPerPageChanged(int rowsPerPage)
    {
        _rowsPerPage = rowsPerPage;
        if (_state is not null)
        {
            _state.Query.Offset = _currentPage * _rowsPerPage;
            _state.Query.Count = _rowsPerPage;
            _state = await testExecutionController.SearchTestCaseRunsAsync(_state.Query, _selectedItem);
            _totalCount = _state.Data.TotalCount;
            _dataGrid?.ReloadServerData();
        }
    }

    private async Task OnCurrentPageChanged(int page)
    {
        _currentPage = page;
        if (_state is not null)
        {
            _state.Query.Offset = _currentPage * _rowsPerPage;
            _state.Query.Count = _rowsPerPage;
            _state = await testExecutionController.SearchTestCaseRunsAsync(_state.Query, _selectedItem);
            _totalCount = _state.Data.TotalCount;
            _dataGrid?.ReloadServerData();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _query.TestRunId = _run?.Id;
            _query.ProjectId = Project.Id;

            // Search for test case runs.
            _state = await testExecutionController.SearchTestCaseRunsAsync(_query, _selectedItem);
            _totalCount = _state.Data.TotalCount;
            this.StateHasChanged();
        }
    }

    private async Task ReloadDataAsync()
    {
        _state = await testExecutionController.SearchTestCaseRunsAsync(_query, _selectedItem);
        _totalCount = _state.Data.TotalCount;
        _dataGrid?.ReloadServerData();
    }

    /// <summary>
    /// Loads data for the grid based on the current state and query.
    /// </summary>
    /// <param name="state">The grid state (paging, sorting, etc.).</param>
    /// <returns>Grid data containing items and total count.</returns>
    private async Task<GridData<TestCaseRun>> LoadGridData(GridState<TestCaseRun> state)
    {
        if(_state is null)
        {
            return new GridData<TestCaseRun>()
            {
                Items = [],
                TotalItems = 0
            };
        }

        return new GridData<TestCaseRun>
        {
            Items = _state.Data.Items,
            TotalItems = (int)_state.Data.TotalCount
        };
    }

    /// <summary>
    /// Re-runs a test case by resetting its result and executing it again.
    /// </summary>
    /// <param name="testCaseRun">The test case run to re-run.</param>
    private async Task RunTestAgain(TestCaseRun testCaseRun)
    {
        if (_selectedItem is null)
        {
            return;
        }
        await testExecutionController.SetTestCaseRunResultAsync(testCaseRun, TestResult.NoRun);

        await testExecutionController.RunTestAsync(testCaseRun);
    }

    /// <summary>
    /// Runs a test case.
    /// </summary>
    /// <param name="testCaseRun">The test case run to execute.</param>
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
        if (Run is not null)
        {
            appNavigationManager.State.SetSelectedTestCaseRun(Run, testCaseRun);
        }
    }
}