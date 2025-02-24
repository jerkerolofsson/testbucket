

using TestBucket.Contracts.Testing.Models;
using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Testing;
internal class TestCaseRepository : ITestCaseRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestCaseRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddTestCaseAsync(TestCase testCase)
    {
        testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestCases.AddAsync(testCase);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string name)
    {
        var folder = new TestSuiteFolder 
        { 
            Name = name, 
            TestSuiteId = testSuiteId,
            Created = DateTimeOffset.UtcNow, 
            TenantId = tenantId, 
            TestProjectId = projectId, 
            ParentId = parentFolderId 
        };

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestSuiteFolders.AddAsync(folder);
        await dbContext.SaveChangesAsync();
        return folder;
    }

    public async Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, long? projectId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var suites = dbContext.TestSuites.Where(x => x.TenantId == tenantId && x.TestProjectId == projectId);

        // Apply filter
        if (query.Text is not null)
        {
            suites = suites.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await suites.LongCountAsync();
        var items = suites.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

        return new PagedResult<TestSuite>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }

    public async Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var folders = dbContext.TestSuiteFolders.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.ParentId == parentFolderId && x.TestSuiteId == testSuiteId)
            .OrderBy(x=>x.Name);
        return await folders.ToArrayAsync();
    }

    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(string tenantId, SearchTestQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases.Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.Text is not null)
        {
            tests = tests.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }
        if (query.FolderId is not null)
        {
            tests = tests.Where(x => x.TestSuiteFolderId == query.FolderId);
        }
        if (query.TestSuiteId is not null)
        {
            tests = tests.Where(x => x.TestSuiteId == query.TestSuiteId);
        }

        var allTests = await tests.ToArrayAsync();

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Created).Skip(query.Offset).Take(query.Count).ToArray();

        return new PagedResult<TestCase>
        {
            TotalCount = totalCount,
            Items = items,
        };
    }

    public async Task<TestSuite> AddTestSuiteAsync(string tenantId, long? projectId, string name)
    {
        var testSuite = new TestSuite { Name = name, Created = DateTimeOffset.UtcNow, TenantId = tenantId, TestProjectId = projectId };

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestSuites.AddAsync(testSuite);
        await dbContext.SaveChangesAsync();
        return testSuite;
    }

    public async Task DeleteFolderByIdAsync(string tenantId, long folderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await DeleteFolderByIdAsync(tenantId, folderId, dbContext);
        await dbContext.SaveChangesAsync();
    }

    private async Task DeleteFolderByIdAsync(string tenantId, long folderId, ApplicationDbContext dbContext)
    {
        foreach (var childFolder in await dbContext.TestSuiteFolders.Where(x => x.TenantId == tenantId && x.ParentId == folderId).ToListAsync())
        {
            await DeleteFolderByIdAsync(tenantId, childFolder.Id, dbContext);
        }

        foreach(var testCase in await dbContext.TestCases.Where(x=>x.TestSuiteFolderId == folderId).ToListAsync())
        {
            foreach(var testCaseRun in await dbContext.TestCaseRuns.Where(x=>x.TestCaseId == testCase.Id && x.TenantId == tenantId).ToListAsync())
            {
                dbContext.TestCaseRuns.Remove(testCaseRun);
            }
            dbContext.TestCases.Remove(testCase);
        }

        var folder = await dbContext.TestSuiteFolders.Where(x => x.TenantId == tenantId && x.Id == folderId).FirstOrDefaultAsync();
        if (folder is not null)
        {
            dbContext.TestSuiteFolders.Remove(folder);
        }
    }

}
