
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Search;

namespace TestBucket.Components.Requirements.Controls;

public partial class RequirementGrid
{
    [Parameter] public TestProject? Project { get; set; }
    [Parameter] public EventCallback<Requirement> OnRequirementClicked { get; set; }

    [Parameter] public SearchRequirementQuery? Query { get; set; }
    [Parameter] public EventCallback<SearchRequirementQuery> QueryChanged { get; set; }

    [Parameter] public EventCallback<long> TotalNumberOfRequirementsChanged { get; set; }


    private SearchRequirementQuery _query = new();

    private MudDataGrid<Requirement?> _dataGrid = default!;

    private bool _hasQueryChanged = false;
    private long? _projectId = null;
    private bool _hasCustomFilter = false;
    private IReadOnlyList<FieldDefinition> _fields = [];
    private string _searchPhrase = "";


    #region Lifecycle


    protected override async Task OnParametersSetAsync()
    {
        if(Query is null)
        {
            return;
        }

        if (Project is not null && _projectId != Project.Id)
        {
            _projectId = Project.Id;
            _fields = await fieldController.GetDefinitionsAsync(_projectId.Value, Contracts.Fields.FieldTarget.TestCase);
        }

        Query.ProjectId = _projectId;

        _hasQueryChanged = !_query.Equals(Query);
        if (_hasQueryChanged)
        {
            _query = Query;
            SetSearchPhraseFromQuery();
            _dataGrid?.ReloadServerData();
        }
    }
    protected override void OnInitialized()
    {
    }
    public void Dispose()
    {
    }
    #endregion
    private string RowClassFunc(Requirement item, int _)
    {
       
        return "tb-datarow cursor-pointer";
    }
    private async Task OnItemClicked(Requirement item)
    {
        await OnRequirementClicked.InvokeAsync(item);
    }

    private async Task ResetFilter()
    {
        _hasQueryChanged = true;
        _hasCustomFilter = false;
        await QueryChanged.InvokeAsync(null);
    }

    private async Task OnSearch(string text)
    {
        _query = SearchRequirementQueryParser.Parse(text, _fields);

        SetSearchPhraseFromQuery();

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

    private void SetSearchPhraseFromQuery()
    {
        _query.FolderId = null;
        _query.ProjectId = null;
        _searchPhrase = _query.ToSearchText();
        _query.CompareFolder = false;
        _query.ProjectId = Project?.Id;
    }

    private async Task<GridData<Requirement>> LoadGridData(GridState<Requirement> state)
    {
        _query.ProjectId = Project?.Id;
        _query.Offset = state.Page * state.PageSize;
        _query.Count = state.PageSize;
        var result = await browser.SearchAsync(_query);

        GridData<Requirement> data = new()
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };

        await TotalNumberOfRequirementsChanged.InvokeAsync(result.TotalCount);

        return data;
    }

}
