using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Automation.Pipelines.Models;
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
    public async Task<PagedResult<Pipeline>> SearchAsync(FilterSpecification<Pipeline>[] filters, int offset, int count)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var pipelines = dbContext.Pipelines.AsNoTracking().AsQueryable();
        foreach (var filter in filters)
        {
            pipelines = pipelines.Where(filter.Expression);
        }

        long total = await pipelines.LongCountAsync();

        return new PagedResult<Pipeline>
        {
            TotalCount = total,
            Items = await pipelines.OrderBy(x => x.Created).Skip(offset).Take(count).ToArrayAsync()
        };
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
