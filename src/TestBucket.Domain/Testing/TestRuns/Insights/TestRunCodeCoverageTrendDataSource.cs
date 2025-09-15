using TestBucket.Contracts.Insights;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Testing.TestRuns.Insights;
internal class TestRunCodeCoverageTrendDataSource : IInsightsDataSource
{
    private readonly ITestRunInsightsRepository _repo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data source
    /// </summary>
    public string DataSource => TestRunDataSourceNames.CodeCoverageTrend;

    public TestRunCodeCoverageTrendDataSource(ITestRunInsightsRepository manager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _repo = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();

        IReadOnlyList<FieldDefinition> fields = fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId);
        var testRunQuery = TestRunQueryParser.Parse(query.Query, fields);
        if (projectId is not null)
        {
            testRunQuery.ProjectId = projectId;
        }
        List<FilterSpecification<TestRun>> filters = TestRunFilterSpecificationBuilder.From(testRunQuery);
        filters.Add(new FilterByTenant<TestRun>(tenantId));


        var data = await _repo.GetCodeCoverageTrendAsync(filters);
        return data.ConvertToStringLabels();
    }
}
