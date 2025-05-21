using TestBucket.Domain.Fields;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.Testing.TestCases.Insights;
internal class CountByCategoryDataSource : IInsightsDataSource
{
    private readonly ITestCaseManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    /// <summary>
    /// Gets the identifier of the data saource
    /// </summary>
    public string DataSource => TestCaseDataSourceNames.CountByCategory;

    public CountByCategoryDataSource(ITestCaseManager manager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        IReadOnlyList<FieldDefinition> fields = [];
        if (projectId is not null)
        {
            fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, Contracts.Fields.FieldTarget.TestCase);
        }
        SearchTestQuery testQuery = SearchTestCaseQueryParser.Parse(query.Query, fields);
        if (projectId is not null)
        {
            testQuery.ProjectId = projectId;
        }
        var data = await _manager.GetInsightsTestCountPerFieldAsync(principal, testQuery, Traits.Core.TraitType.TestCategory);
        var stringLabelData = data.ConvertToStringLabels();
        stringLabelData.Series[0].SortBy = InsightsSort.ValueDescending;
        stringLabelData.Series[0].Name = "count-per-testcategory";
        return stringLabelData;
    }
}
