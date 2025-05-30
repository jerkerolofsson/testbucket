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

/// <summary>
/// Counts each result by the latest run of each test case
/// </summary>
internal class CountByLatestResultDataSource : IInsightsDataSource
{
    private readonly ITestRunManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data source
    /// </summary>
    public string DataSource => TestRunDataSourceNames.CountByLatestResult;

    public CountByLatestResultDataSource(ITestRunManager manager, IFieldDefinitionManager fieldDefinitionManager)
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
        var data = await _manager.GetInsightsLatestTestResultsAsync(principal, testRunQuery);

        string[] results = [TestResult.Passed.ToString(), TestResult.Failed.ToString(), TestResult.Blocked.ToString(), TestResult.Skipped.ToString(), TestResult.NoRun.ToString()];

        var stringLabelData = data.ConvertToStringLabels();
        stringLabelData.AddMissingLabels(results);
        return stringLabelData;
    }
}
