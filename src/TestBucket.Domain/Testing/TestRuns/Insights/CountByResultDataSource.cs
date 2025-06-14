using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Extensions;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.TestRuns.Insights;
internal class CountByResultDataSource : IInsightsDataSource
{
    private readonly ITestRunManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data source
    /// </summary>
    public string DataSource => TestRunDataSourceNames.CountByResult;

    public CountByResultDataSource(ITestRunManager manager, IFieldDefinitionManager fieldDefinitionManager)
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
        var data = await _manager.GetInsightsTestResultsAsync(principal, testRunQuery);

        string[] results = [TestResult.Passed.ToString(), TestResult.Failed.ToString(), TestResult.Blocked.ToString(), TestResult.Skipped.ToString(), TestResult.NoRun.ToString()];

        var stringLabelData = data.ConvertToStringLabels();
        stringLabelData.AddMissingLabels(results);
        return stringLabelData;
    }
}
