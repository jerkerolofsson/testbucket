using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.TestResources;

namespace TestBucket.Data.TestResources;
internal class TestResourceRepository : ITestResourceRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestResourceRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(TestResource resource)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestResources.Add(resource);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestResources.Where(x => x.Id == id).ExecuteDeleteAsync();
    }
    public async Task<TestResource?> GetByIdAsync(string tenantId, long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestResources.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TestResource>> SearchAsync(FilterSpecification<TestResource>[] filters, int offset, int count)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var resources = dbContext.TestResources.AsNoTracking().AsQueryable();

        foreach (var filter in filters)
        {
            resources = resources.Where(filter.Expression);
        }

        var totalCount = await resources.LongCountAsync();

        return new PagedResult<TestResource>
        {
            TotalCount = totalCount,
            Items = await resources.Skip(offset).Take(count).ToArrayAsync()
        };
    }

    public async Task UpdateAsync(TestResource resource)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestResources.Update(resource);
        await dbContext.SaveChangesAsync();
    }
}
