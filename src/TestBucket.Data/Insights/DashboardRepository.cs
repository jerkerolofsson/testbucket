using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Data.Insights;
internal class DashboardRepository : IDashboardRepository
{

    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public DashboardRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddDashboardAsync(Dashboard dashboard)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.Dashboards.Add(dashboard);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteDashboardAsync(string tenantId, long id)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var dashboard = await dbContext.Dashboards.FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);
        if (dashboard is not null)
        {
            dbContext.Dashboards.Remove(dashboard);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Dashboard>> GetAllDashboardsAsync(long projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Dashboards.Where(d => d.TestProjectId == projectId).ToListAsync();
    }

    public async Task<Dashboard?> GetDashboardAsync(string tenantId, long id)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Dashboards.FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);
    }

    public async Task<Dashboard?> GetDashboardByNameAsync(string tenantId, long projectId, string name)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Dashboards.FirstOrDefaultAsync(d => d.Name == name && d.TenantId == tenantId && d.TestProjectId == projectId);
    }

    public async Task UpdateDashboardAsync(Dashboard dashboard)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.Dashboards.Update(dashboard);
        await dbContext.SaveChangesAsync();
    }
}
