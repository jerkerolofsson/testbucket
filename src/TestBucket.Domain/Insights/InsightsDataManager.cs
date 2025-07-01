using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights;
internal class InsightsDataManager : IInsightsDataManager
{
    private readonly Dictionary<string, IInsightsDataSource> _dataSources = [];

    public InsightsDataManager(IEnumerable<IInsightsDataSource> dataSources)
    {
        foreach (var dataSource in dataSources)
        {
            _dataSources[dataSource.DataSource] = dataSource;
        }
    }

    public async Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query)
    {
        if (_dataSources.TryGetValue(query.DataSource, out var source))
        {
            return await source.GetDataAsync(principal, projectId, query);
        }
        return new InsightsData<string, double>();
        //throw new InvalidOperationException($"The data source '{query.DataSource}' is not registered.");
    }

    public string[] GetDataSourceNames()
    {
        return _dataSources.Keys.ToArray();
    }
}
