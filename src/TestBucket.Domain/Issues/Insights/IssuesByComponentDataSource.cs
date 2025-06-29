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
internal class IssuesByComponentDataSource : IInsightsDataSource
{
    private readonly IIssueManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data saource
    /// </summary>
    public string DataSource => IssueDataSourceNames.IssuesByComponent;

    public IssuesByComponentDataSource(IIssueManager manager, IFieldDefinitionManager fieldDefinitionManager, IStateService stateService)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        IReadOnlyList<FieldDefinition> fields = [];
        if (projectId is not null)
        {
            fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, Contracts.Fields.FieldTarget.Issue);
        }
        var componentFieldDefinition = fields.Where(x => x.TraitType == Traits.Core.TraitType.Component).FirstOrDefault();
        if(componentFieldDefinition is null)
        {
            // Empty
            return new();
        }

        SearchIssueQuery issueQuery = SearchIssueRequestParser.Parse(projectId, query.Query, fields);
        if (projectId is not null)
        {
            issueQuery.ProjectId = projectId;
        }
        var data = await _manager.GetIssueCountPerFieldAsync(principal, issueQuery, componentFieldDefinition.Id);

        return data.ConvertToStringLabels();
    }
}
