
using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Data.Runners
{
    class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public JobRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(Job job)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.Jobs.AddAsync(job);
            await dbContext.SaveChangesAsync();
        }


        public async Task<Job?> GetByGuidAsync(string guid)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Jobs.AsNoTracking().Where(x => x.Guid == guid).FirstOrDefaultAsync();
        }

        public async Task<Job?> GetByIdAsync(long id)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Jobs.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Job?> GetOneAsync(string tenantId, long? projectId, PipelineJobStatus status, string[] languages)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var jobs = dbContext.Jobs.AsNoTracking().Where(x => x.Status == status && x.TenantId == tenantId);
            jobs = jobs.Where(x => languages.Contains(x.Language));

            if(projectId is not null)
            {
                jobs = jobs.Where(x => x.TestProjectId == projectId);
            }


            return await jobs.OrderByDescending(x=>x.Priority).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Jobs.Update(job);
            await dbContext.SaveChangesAsync();
        }
    }
}
