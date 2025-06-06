
using TestBucket.Domain.Metrics;
using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Data.Metrics;
internal class MetricsRepository : IMetricsRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public MetricsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddMetric(Metric metric)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Metrics.Add(metric);
        await dbContext.SaveChangesAsync();
    }


    public async Task DeleteMetric(Metric metric)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Metrics.Remove(metric);
        await dbContext.SaveChangesAsync();
    }
}
