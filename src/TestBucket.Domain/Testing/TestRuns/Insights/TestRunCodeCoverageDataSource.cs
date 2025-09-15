using TestBucket.Contracts.Insights;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.TestRuns.Insights;
internal class TestRunCodeCoverageDataSource : IInsightsDataSource
{
    private readonly ITestRunInsightsRepository _repo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data source
    /// </summary>
    public string DataSource => TestRunDataSourceNames.CodeCoverage;

    public TestRunCodeCoverageDataSource(ITestRunInsightsRepository manager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _repo = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);

        double? percent = null;

        IReadOnlyList<FieldDefinition> fields = [];
        SearchTestCaseRunQuery testRunQuery = SearchTestCaseRunQueryParser.Parse(query.Query, fields);
        if(testRunQuery.TestRunId is not null)
        {
            percent = await _repo.GetCodeCoverageAsync(testRunQuery.TestRunId.Value);
        }

        InsightsData<string, double> data = new InsightsData<string, double>();
        if (percent is not null)
        {
            data.Add("Code Coverage").Add("Code Coverage", Math.Round(percent.Value,1));
        }
        else
        {
            data.Add("Code Coverage").Add("Code Coverage", 0.0);
        }
        return data;
    }
}
