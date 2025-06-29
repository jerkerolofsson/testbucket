using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights;
public interface IDashboardRepository
{
    Task<Dashboard?> GetDashboardAsync(string tenantId, long id);
    Task<IEnumerable<Dashboard>> GetAllDashboardsAsync(long projectId);
    Task AddDashboardAsync(Dashboard dashboard);
    Task UpdateDashboardAsync(Dashboard dashboard);
    Task DeleteDashboardAsync(string tenantId, long id);
    Task<Dashboard?> GetDashboardByNameAsync(string tenantId, long projectId, string name);
}
