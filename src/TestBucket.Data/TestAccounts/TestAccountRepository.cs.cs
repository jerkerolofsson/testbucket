using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Data.TestAccounts;
internal class TestAccountRepository : ITestAccountRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestAccountRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(TestAccount account)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestAccounts.Add(account);    
        await dbContext.SaveChangesAsync();
    }

    public async Task<TestAccount?> GetAccountByIdAsync(string tenantId, long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestAccounts.AsNoTracking().Where(x => x.Id == id && x.TenantId == tenantId).FirstOrDefaultAsync();
    }
    public async Task DeleteAsync(long id)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestAccounts.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public async Task<PagedResult<TestAccount>> SearchAsync(FilterSpecification<Domain.TestAccounts.Models.TestAccount>[] filters, int offset, int count)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var accounts = dbContext.TestAccounts.AsNoTracking().AsQueryable();

        foreach(var filter in filters)
        {
            accounts = accounts.Where(filter.Expression);
        }

        var totalCount = await accounts.LongCountAsync();

        return new PagedResult<TestAccount>
        {
            TotalCount = totalCount,
            Items = await accounts.Skip(offset).Take(count).ToArrayAsync()
        };
    }

    public async Task UpdateAsync(TestAccount account)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestAccounts.Update(account);
        await dbContext.SaveChangesAsync();
    }
}
