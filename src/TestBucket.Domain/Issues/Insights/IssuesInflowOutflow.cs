using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Insights;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Extensions;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.States;

namespace TestBucket.Domain.Issues.Insights;
internal class IssuesInflowOutflow : IInsightsDataSource
{
    private readonly IIssueManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IStateService _stateService;

    /// <summary>
    /// Gets the identifier of the data saource
    /// </summary>
    public string DataSource => IssueDataSourceNames.IssuesInflowOutflow;

    public IssuesInflowOutflow(IIssueManager manager, IFieldDefinitionManager fieldDefinitionManager, IStateService stateService)
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
        var created = await _manager.GetCreatedIssuesPerDay(principal, issueQuery);
        var closed = await _manager.GetClosedIssuesPerDay(principal, issueQuery);

        var data = new InsightsData<DateOnly, int>() { Name = "issues-inflow-outflow" };
        data.Add(created.Series[0]);
        data.Add(closed.Series[0]);
        data.AddMissingDays();
        return data.ConvertToStringLabels();
    }
}
