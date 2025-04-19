

using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Data.Runners
{
    class RunnerRepository : IRunnerRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public RunnerRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(Runner runner)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.Runners.AddAsync(runner);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Runner runner)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Runners.Remove(runner);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Runner>> GetAllAsync(string tenantId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Runners.AsNoTracking().Where(x => x.TenantId == tenantId).ToListAsync();
        }

        public async Task<Runner?> GetByIdAsync(string id)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Runners.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Runner runner)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Runners.Update(runner);
            await dbContext.SaveChangesAsync();
        }
    }
}
