using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Extensions;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.TestRuns.Insights;
internal class ResultsByComponentDataSource : IInsightsDataSource
{
    private readonly ITestRunManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data source
    /// </summary>
    public string DataSource => TestRunDataSourceNames.ResultsByComponent;

    public ResultsByComponentDataSource(ITestRunManager manager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        IReadOnlyList<FieldDefinition> fields = [];
        if (projectId is not null)
        {
            fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, Contracts.Fields.FieldTarget.TestCaseRun);
        }
        SearchTestCaseRunQuery testRunQuery = SearchTestCaseRunQueryParser.Parse(query.Query, fields);
        if (projectId is not null)
        {
            testRunQuery.ProjectId = projectId;
        }

        var field = fields.Where(x => !x.IsDeleted && x.TraitType == Traits.Core.TraitType.Component).FirstOrDefault();
        if (field is null)
        {
            return new InsightsData<string, double>();
        }

        var data = await _manager.GetInsightsTestResultsByFieldAsync(principal, testRunQuery, field.Id);

        var stringLabelData = data.ConvertToStringLabels();
        return stringLabelData;
    }
}
