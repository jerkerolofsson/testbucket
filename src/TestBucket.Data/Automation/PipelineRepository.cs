
using TestBucket.Domain.Automation;
using TestBucket.Domain.Automation.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Automation;
internal class PipelineRepository : IPipelineRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public PipelineRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(Pipeline pipeline)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Pipelines.AddAsync(pipeline);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateAsync(Pipeline pipeline)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Pipelines.Update(pipeline);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Pipelines.Where(x => x.Id == id).ExecuteDeleteAsync();
    }
    public async Task DeleteAsync(Pipeline pipeline)
    {
        await DeleteAsync(pipeline.Id);
    }

    public async Task<Pipeline?> GetByIdAsync(long pipelineId)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Pipelines.AsNoTracking().Include(x=>x.PipelineJobs).Where(x => x.Id == pipelineId).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(FilterSpecification<Pipeline>[] filters, long testRunId)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var pipelines = dbContext.Pipelines.AsNoTracking().AsQueryable();
        foreach (var filter in filters)
        {
            pipelines = pipelines.Where(filter.Expression);
        }
        return await pipelines.ToListAsync();
    }
}
