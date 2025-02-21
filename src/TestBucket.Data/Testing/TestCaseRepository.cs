

using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Data.Testing;
internal class TestCaseRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestCaseRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<PagedResult<TestCase>> GetTestCasesAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases.Where(x => x.TenantId == tenantId);

        // Apply filter
        if(query.Text is not null)
        {
            tests = tests.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Created).Skip(query.Offset).Take(query.Count);

        return new PagedResult<TestCase>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }
}
