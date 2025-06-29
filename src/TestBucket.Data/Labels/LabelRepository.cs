using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Labels;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Labels;
internal class LabelRepository : ILabelRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public LabelRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddLabelAsync(Label label)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Labels.Add(label);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteLabelByIdAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Labels.AsNoTracking().Where(x => x.Id == id).ExecuteDeleteAsync();    
    }

    public async Task<Label?> GetLabelByIdAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Labels.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Label>> GetLabelsAsync(IEnumerable<FilterSpecification<Label>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var labels = dbContext.Labels.AsNoTracking().AsQueryable();
        foreach(var filter in filters)
        {
            labels = labels.Where(filter.Expression);
        }
        return await labels.OrderBy(x=>x.Title).ToListAsync();
    }

    public async Task UpdateLabelAsync(Label label)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Labels.Update(label);
        await dbContext.SaveChangesAsync();
    }
}
