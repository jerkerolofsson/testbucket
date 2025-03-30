using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Migrations;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Data.TestEnvironments;
internal class TestEnvironmentRepository : ITestEnvironmentRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestEnvironmentRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddTestEnvironmentAsync(TestEnvironment testEnvironment)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestEnvironments.Add(testEnvironment);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTestEnvironmentAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestEnvironments.Where(x => x.Id == id).ExecuteDeleteAsync();
    }
    public async Task<TestEnvironment?> GetTestEnvironmentByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestEnvironments.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }
    public async Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(string tenantId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestEnvironments.AsNoTracking().Where(x => x.TenantId == tenantId).ToListAsync();
    }

    public async Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(IEnumerable<FilterSpecification<TestEnvironment>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var environments = dbContext.TestEnvironments.AsNoTracking().AsQueryable();
        foreach(var filter in filters)
        {
            environments = environments.Where(filter.Expression);
        }
        return await environments.ToListAsync();
    }

    public async Task UpdateTestEnvironmentAsync(TestEnvironment testEnvironment)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestEnvironments.Update(testEnvironment);
        await dbContext.SaveChangesAsync();
    }
}
