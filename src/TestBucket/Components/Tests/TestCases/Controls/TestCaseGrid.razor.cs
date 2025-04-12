
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

    private TestSuiteFolder? _folder;

    private SearchTestQuery _query = new();

    private MudDataGrid<TestSuiteItem?> _dataGrid = default!;
    private IReadOnlyList<FieldDefinition> _definitions = [];

    private List<FieldDefinition> _columns = [];

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
    private long? _testProjectId = null;
    private TestSuite? _testSuite;

    protected override async Task OnParametersSetAsync()
    {
        bool changed = false;

        if(_testSuiteId != TestSuiteId)
        {
            _testSuiteId = TestSuiteId;
            _testSuite = null;
            changed = true;
            if (_testSuiteId is not null)
            {
                _testSuite = await testSuiteServer.GetTestSuiteByIdAsync(_testSuiteId.Value);
                _testProjectId = _testSuite?.TestProjectId;
            }

        }
        if (_folderId != FolderId)
        {
            _folderId = FolderId;
            _folder = null;
            changed = true;
            if (_folderId is not null)
            {
                _folder = await testSuiteServer.GetTestSuiteFolderByIdAsync(_folderId.Value);
                _testProjectId = _folder?.TestProjectId;
            }
        }
        else if(Folder is not null)
        {
            if(_folder?.Id != Folder.Id)
            {
                changed = true;
                _folder = Folder;
                _testProjectId = _folder.TestProjectId;
            }
        }

        if (changed)
        {
            _dataGrid?.ReloadServerData();
        }

        _columns = [];
        //if(testProjectId is not null)
        //{
        //    _definitions = await fieldController.SearchDefinitionsAsync(new SearchFieldQuery { ProjectId = testProjectId, Target = FieldTarget.TestCase, Count = 100 });

        //    var category = _definitions.FirstOrDefault(x => x.TraitType == Traits.Core.TraitType.TestCategory);
        //    if (category is not null && !_columns.Any(x => x.TraitType == Traits.Core.TraitType.TestCategory))
        //    {
        //        _columns.Add(category);
        //    }
        //    var activity = _definitions.FirstOrDefault(x => x.TraitType == Traits.Core.TraitType.TestActivity);
        //    if (activity is not null && !_columns.Any(x => x.TraitType == Traits.Core.TraitType.TestActivity))
        //    {
        //        _columns.Add(activity);
        //    }
        //    var prio = _definitions.FirstOrDefault(x => x.TraitType == Traits.Core.TraitType.TestPriority);
        //    if (prio is not null && !_columns.Any(x => x.TraitType == Traits.Core.TraitType.TestPriority))
        //    {
        //        _columns.Add(prio);
        //    }
        //}
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

    private async Task ShowFilterAsync()
    {
        var parameters = new DialogParameters<EditTestCaseFilterDialog>
        {
            { x => x.Query, _query },
            { x => x.Project, Project },
        };
        var dialog = await dialogService.ShowAsync<EditTestCaseFilterDialog>("Filter Tests", parameters);
        var result = await dialog.Result;
        if (result?.Data is SearchTestQuery query)
        {
            _query = query;
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
        _query.Text = text;
        _dataGrid?.ReloadServerData();
    }

    private async Task<GridData<TestSuiteItem>> LoadGridData(GridState<TestSuiteItem> state)
    {
        var query = new SearchTestQuery
        {
            CompareFolder = CompareFolder,
            TestSuiteId = TestSuiteId,
            FolderId = _folder?.Id,
            CreatedFrom = _query.CreatedFrom,
            CreatedUntil = _query.CreatedUntil,
            Category = _query.Category,
            Priority = _query.Priority,
            ProjectId = Project?.Id,

            Text = string.IsNullOrWhiteSpace(_query.Text) ? null : _query.Text,
        };

        var result = await testBrowser.SearchItemsAsync(query, state.Page * state.PageSize, state.PageSize);

        GridData<TestSuiteItem> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        return data;
    }

}
