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
internal class CountByResultDataSource : IInsightsDataSource
{
    private readonly ITestRunManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IStateService _stateService;

    /// <summary>
    /// Gets the identifier of the data saource
    /// </summary>
    public string DataSource => TestRunDataSourceNames.CountByResult;

    public CountByResultDataSource(ITestRunManager manager, IFieldDefinitionManager fieldDefinitionManager, IStateService stateService)
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
        SearchTestCaseRunQuery issueQuery = SearchTestCaseRunQueryParser.Parse(query.Query, fields);
        if (projectId is not null)
        {
            issueQuery.ProjectId = projectId;
        }
        var data = await _manager.GetInsightsTestResultsAsync(principal, issueQuery);

        string[] results = [TestResult.Passed.ToString(), TestResult.Failed.ToString(), TestResult.Blocked.ToString(), TestResult.Skipped.ToString(), TestResult.NoRun.ToString()];

        var stringLabelData = data.ConvertToStringLabels();
        stringLabelData.AddMissingLabels(results);
        return stringLabelData;
    }
}
