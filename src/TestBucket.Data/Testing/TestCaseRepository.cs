
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

        await LookupProjectIdFromSuiteId(testCase, dbContext);
        await CalculatePathAsync(dbContext, testCase);

        await dbContext.TestCases.AddAsync(testCase);
        await dbContext.SaveChangesAsync();

        await GenerateTestSlugIfMissingAsync(testCase, dbContext);
    }

    private static async Task LookupProjectIdFromSuiteId(TestCase testCase, ApplicationDbContext dbContext)
    {
        if (testCase.TestProjectId is null && testCase.TestSuiteId > 0)
        {
            var testSuite = await dbContext.TestSuites.AsNoTracking().Where(x => x.Id == testCase.TestSuiteId).FirstOrDefaultAsync();
            if (testSuite is not null)
            {
                testCase.TestProjectId = testSuite.TestProjectId;
            }
        }
    }

    private static async Task GenerateTestSlugIfMissingAsync(TestCase testCase, ApplicationDbContext dbContext)
    {
        if (string.IsNullOrEmpty(testCase.Slug))
        {
            testCase.Slug = "TC-" + testCase.Id;
            if (testCase.TestProjectId is not null)
            {
                var project = dbContext.Projects.AsNoTracking().Where(x => x.Id == testCase.TestProjectId).FirstOrDefault();
                if (project is not null)
                {
                    testCase.Slug = project.ShortName + "-" + testCase.Id;
                }
            }

            dbContext.TestCases.Update(testCase);
            await dbContext.SaveChangesAsync();
        }
    }


    public async Task UpdateTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var existingFolder = await dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.Id == folder.Id).FirstOrDefaultAsync();
        if(existingFolder is null)
        {
            throw new InvalidOperationException("Folder does not exist!");
        }

        var hasPathChanged = existingFolder.Name != folder.Name;
        if(hasPathChanged)
        {
            // todo: process all tests
        }

        dbContext.TestSuiteFolders.Update(folder);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTestCaseByIdAsync(long id)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await foreach (var run in dbContext.TestCaseRuns.Where(x => x.TestCaseId == id).AsAsyncEnumerable())
        {
            dbContext.TestCaseRuns.Remove(run);
        }

        await foreach (var field in dbContext.TestCaseFields.Where(x => x.TestCaseId == id).AsAsyncEnumerable())
        {
            dbContext.TestCaseFields.Remove(field);
        }

        await foreach (var test in dbContext.TestCases.Where(x=>x.Id == id).AsAsyncEnumerable())
        {
            dbContext.TestCases.Remove(test);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateTestCaseAsync(TestCase testCase)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await CalculatePathAsync(dbContext, testCase);

        dbContext.TestCases.Update(testCase);
        await dbContext.SaveChangesAsync();

        await GenerateTestSlugIfMissingAsync(testCase, dbContext);
    }

    private async Task CalculatePathAsync(ApplicationDbContext dbContext, TestCase testCase)
    {
        List<string> pathComponents = new();
        await CalculatePathAsync(dbContext, testCase.TestSuiteFolderId, pathComponents);
        pathComponents.Reverse();
        testCase.Path = string.Join('/', pathComponents);
    }

    private async Task CalculatePathAsync(ApplicationDbContext dbContext, long? folderId, List<string> pathComponents)
    {
        while (folderId is not null)
        {
            var folder = await dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.Id == folderId).FirstOrDefaultAsync();
            if (folder is null)
            {
                return;
            }
            pathComponents.Add(folder.Name);
            folderId = folder.ParentId;
        }
    }

    public async Task<TestSuiteFolder?> GetTestSuiteFolderByNameAsync(string tenantId, long testSuiteId, long? parentId, string folderName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var folders = dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.TenantId == tenantId && x.TestSuiteId == testSuiteId && x.ParentId == parentId && x.Name == folderName);
        return await folders.FirstOrDefaultAsync();
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

    public async Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var suites = dbContext.TestSuites.Where(x => x.TenantId == tenantId && x.TestProjectId == query.ProjectId);

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


    public async Task UpdateTestSuiteAsync(TestSuite suite)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.TestSuites.Update(suite);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TestSuite> AddTestSuiteAsync(string tenantId, long? teamId, long? projectId, string name)
    {
        var testSuite = new TestSuite 
        { 
            Name = name, 
            Created = DateTimeOffset.UtcNow, 
            TeamId = teamId,
            TenantId = tenantId, 
            TestProjectId = projectId 
        };

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

    public async Task<TestSuite?> GetTestSuiteByNameAsync(string tenantId, long? teamId, long? projectId, string suiteName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestSuites.AsNoTracking()
            .Where(x =>
            x.TenantId == tenantId &&
            x.TeamId == teamId &&
            x.TestProjectId == projectId && 
            x.Name == suiteName).FirstOrDefaultAsync();
    }

    public async Task AddTestRunAsync(TestRun testRun)
    {
        testRun.Created = DateTimeOffset.UtcNow;

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestRuns.AddAsync(testRun);
    }

    public async Task<TestCase?> GetTestCaseByExternalIdAsync(string tenantId, long testSuiteId, string? externalId)
    {
        if(externalId is null)
        {
            return null;
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCases.AsNoTracking().Where(x => x.TenantId == tenantId && x.TestSuiteId == testSuiteId && x.ExternalId == externalId).FirstOrDefaultAsync();
    }

    public async Task AddTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        testCaseRun.Created = DateTimeOffset.UtcNow;

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestCaseRuns.AddAsync(testCaseRun);
    }

    public async Task DeleteTestSuiteByIdAsync(string tenantId, long testSuiteId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        foreach(var folder in dbContext.TestSuiteFolders.Where(x=>x.ParentId == null && x.TestSuiteId == testSuiteId))
        {
            await DeleteFolderByIdAsync(tenantId, folder.Id, dbContext);
        }
        await dbContext.TestSuites.Where(x => x.Id == testSuiteId).ExecuteDeleteAsync();
    }
}
