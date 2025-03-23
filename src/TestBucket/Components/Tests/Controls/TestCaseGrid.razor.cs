
using Microsoft.CodeAnalysis;

using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing;

namespace TestBucket.Components.Tests.Controls;

public partial class TestCaseGrid
{
    [Parameter] public bool CompareFolder { get; set; } = true;
    [Parameter] public TestSuiteFolder? Folder { get; set; }
    [Parameter] public long? FolderId { get; set; }
    [Parameter] public TestProject? Project { get; set; }
    [Parameter] public long? TestSuiteId { get; set; }
    [Parameter] public EventCallback<TestCase> OnTestCaseClicked { get; set; }

    private TestSuiteFolder? _folder;

    private SearchTestQuery _query = new();

    private MudDataGrid<TestCase?> _dataGrid = default!;
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

    protected override async Task OnParametersSetAsync()
    {
        long? testProjectId = null;
        if(TestSuiteId is not null)
        {
            var testSuite = await testSuiteServer.GetTestSuiteByIdAsync(TestSuiteId.Value);
            testProjectId = testSuite?.TestProjectId;

        }
        if (FolderId is not null)
        {
            if(FolderId != _folder?.Id)
            {
                _folder = await testSuiteServer.GetTestSuiteFolderByIdAsync(FolderId.Value);
                testProjectId = _folder?.TestProjectId;
                _dataGrid?.ReloadServerData();
            }
        }
        else if(Folder is not null)
        {
            if(_folder?.Id != Folder.Id)
            {
                _folder = Folder;
                testProjectId = _folder.TestProjectId;
                _dataGrid?.ReloadServerData();
            }
        }

        _columns = [];
        if(testProjectId is not null)
        {
            _definitions = await fieldController.SearchDefinitionsAsync(new SearchFieldQuery { ProjectId = testProjectId, Target = FieldTarget.TestCase, Count = 100 });

            var category = _definitions.FirstOrDefault(x => x.TraitType == Traits.Core.TraitType.TestCategory);
            if (category is not null && !_columns.Any(x => x.TraitType == Traits.Core.TraitType.TestCategory))
            {
                _columns.Add(category);
            }
            var activity = _definitions.FirstOrDefault(x => x.TraitType == Traits.Core.TraitType.TestActivity);
            if (activity is not null && !_columns.Any(x => x.TraitType == Traits.Core.TraitType.TestActivity))
            {
                _columns.Add(activity);
            }
            var prio = _definitions.FirstOrDefault(x => x.TraitType == Traits.Core.TraitType.TestPriority);
            if (prio is not null && !_columns.Any(x => x.TraitType == Traits.Core.TraitType.TestPriority))
            {
                _columns.Add(prio);
            }
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
        _query.Text = text;
        _dataGrid?.ReloadServerData();
    }

    private async Task<GridData<TestCase>> LoadGridData(GridState<TestCase> state)
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

            Text = string.IsNullOrWhiteSpace(_query.Text) ? null : _query.Text,
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
