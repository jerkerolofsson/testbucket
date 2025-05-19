
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights;
public interface IInsightsDataManager
{
    /// <summary>
    /// Returns data for the provided data source
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<InsightsData<string, double>> GetDataAsync(ClaimsPrincipal principal, long? projectId, InsightsDataQuery query);
}