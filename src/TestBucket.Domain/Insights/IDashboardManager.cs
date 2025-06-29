using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights;

public interface IDashboardManager
{
    Task AddDashboardAsync(ClaimsPrincipal principal, Dashboard dashboard);
    Task DeleteDashboardAsync(ClaimsPrincipal principal, long id);
    Task<IEnumerable<Dashboard>> GetAllDashboardsAsync(ClaimsPrincipal principal, long projectId);
    Task<Dashboard?> GetDashboardAsync(ClaimsPrincipal principal, long id);
    Task<Dashboard?> GetDashboardByNameAsync(ClaimsPrincipal principal, long projectId, string name);
    Task UpdateDashboardAsync(ClaimsPrincipal principal, Dashboard dashboard);
}