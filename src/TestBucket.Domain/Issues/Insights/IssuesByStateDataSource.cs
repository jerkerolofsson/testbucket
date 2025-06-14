using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Extensions;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.States;

namespace TestBucket.Domain.Issues.Insights;
internal class IssuesByStateDataSource : IInsightsDataSource
{
    private readonly IIssueManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IStateService _stateService;

    /// <summary>
    /// Gets the identifier of the data saource
    /// </summary>
    public string DataSource => IssueDataSourceNames.IssuesByState;

    public IssuesByStateDataSource(IIssueManager manager, IFieldDefinitionManager fieldDefinitionManager, IStateService stateService)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
        _stateService = stateService;
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        IReadOnlyList<FieldDefinition> fields = [];
        if (projectId is not null)
        {
            fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, Contracts.Fields.FieldTarget.Issue);
        }
        SearchIssueQuery issueQuery = SearchIssueRequestParser.Parse(projectId, query.Query, fields);
        if (projectId is not null)
        {
            issueQuery.ProjectId = projectId;
        }
        var data = await _manager.GetIssueCountPerStateAsync(principal, issueQuery);

        if (projectId is not null)
        {
            var states = await _stateService.GetIssueStatesAsync(principal, projectId.Value);

            // Convert the known state to the user defined string
            Func<MappedIssueState, string> converter = (MappedIssueState state) =>
            {
                return states.Where(x => x.MappedState == state).Select(x => x.Name).FirstOrDefault() ?? "-";
            };

            var stringLabelData = data.ConvertToStringLabels(converter);
            stringLabelData.AddMissingLabels(states.Select(x => x.Name));
            return stringLabelData;
        }
        else
        {
            return data.ConvertToStringLabels();
        }
    }
}
