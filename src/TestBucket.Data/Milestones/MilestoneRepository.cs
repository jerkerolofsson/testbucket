using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Migrations;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Milestones;
internal class MilestoneRepository : IMilestoneRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public MilestoneRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddMilestoneAsync(Milestone milestone)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Milestones.Add(milestone);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteMilestoneByIdAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Milestones.AsNoTracking().Where(x => x.Id == id).ExecuteDeleteAsync();    
    }

    public async Task<Milestone?> GetMilestoneByIdAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Milestones.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Milestone>> GetMilestonesAsync(IEnumerable<FilterSpecification<Milestone>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var milestones = dbContext.Milestones.AsNoTracking().AsQueryable();
        foreach(var filter in filters)
        {
            milestones = milestones.Where(filter.Expression);
        }
        return await milestones.ToListAsync();
    }

    public async Task UpdateMilestoneAsync(Milestone milestone)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Milestones.Update(milestone);
        await dbContext.SaveChangesAsync();
    }
}
