using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights;
public interface IInsightsDataSource
{
    /// <summary>
    /// Identifier for the data source
    /// </summary>
    string DataSource { get; }

    /// <summary>
    /// Returns data for a specific data source
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query);
}
