using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Testing;
internal class TestCaseRepository : ITestCaseRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestCaseRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region Test Cases


    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases.Include(x => x.TestCaseFields).AsQueryable();
        foreach (var spec in filters)
        {
            tests = tests.Where(spec.Expression);
        }
        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Created).Skip(offset).Take(count).ToArray();

        return new PagedResult<TestCase>
        {
            TotalCount = totalCount,
            Items = items,
        };
    }

    public async Task<long[]> SearchTestCaseIdsAsync(IEnumerable<FilterSpecification<TestCase>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases.Include(x => x.TestCaseFields).AsQueryable();
        foreach (var spec in filters)
        {
            tests = tests.Where(spec.Expression);
        }
        var items = tests.Select(x=>x.Id).ToArray();
        return items;
    }
    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(string tenantId, SearchTestQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases.Include(x=>x.TestCaseFields).Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.TeamId is not null)
        {
            //tests = tests.Where(x => x.TeamId == query.TeamId);
        }
        if (query.ProjectId is not null)
        {
            tests = tests.Where(x => x.TestProjectId == query.ProjectId);
        }
        if (query.Text is not null)
        {
            tests = tests.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }
        if (query.CompareFolder)
        {
            tests = tests.Where(x => x.TestSuiteFolderId == query.FolderId);
        }
        if (query.TestSuiteId is not null)
        {
            tests = tests.Where(x => x.TestSuiteId == query.TestSuiteId);
        }

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Created).Skip(query.Offset).Take(query.Count).ToArray();

        return new PagedResult<TestCase>
        {
            TotalCount = totalCount,
            Items = items,
        };
    }

    /// <inheritdoc/>
    public async Task<TestCase?> GetTestCaseByIdAsync(string tenantId, long testCaseId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCases.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == testCaseId).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<TestCase?> GetTestCaseByExternalIdAsync(string tenantId, string? externalId)
    {
        if (externalId is null)
        {
            return null;
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCases.AsNoTracking().Where(x => x.TenantId == tenantId && x.ExternalId == externalId).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<TestCase?> GetTestCaseByAutomationImplementationAttributesAsync(string tenantId, string? assemblyName, string? module, string? className, string? method)
    {
        if (assemblyName is null && module is null && className is null && method is null)
        {
            return null;
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCases.AsNoTracking()
            .Where(x => x.TenantId == tenantId && 
            x.AutomationAssembly == assemblyName &&
            x.Module == module &&
            x.ClassName == className &&
            x.Method == method
            ).FirstOrDefaultAsync();
    }


    /// <inheritdoc/>
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

    public async Task DeleteTestCaseByIdAsync(long id)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await foreach (var link in dbContext.RequirementTestLinks.Where(x => x.TestCaseId == id).AsAsyncEnumerable())
        {
            dbContext.RequirementTestLinks.Remove(link);
        }

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

    #endregion Test Cases

    #region Test Suite Folders

    /// <inheritdoc/>
    public async Task<TestSuiteFolder?> GetTestSuiteFolderByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets a test suite folder by name
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <param name="testSuiteId"></param>
    /// <param name="parentFolderId">Parent location</param>
    /// <param name="folderName">Name of folder</param>
    /// <returns></returns>
    public async Task<TestSuiteFolder?> GetTestSuiteFolderByNameAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string folderName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existingFolder = await dbContext.TestSuiteFolders.AsNoTracking()
            .Where(x => x.TenantId == tenantId && 
            x.TestProjectId == projectId && 
            x.TestSuiteId == testSuiteId && 
            x.ParentId == parentFolderId && x.Name == folderName).FirstOrDefaultAsync();
        return existingFolder;
    }

    public async Task UpdateTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var existingFolder = await dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.Id == folder.Id).FirstOrDefaultAsync();
        if (existingFolder is null)
        {
            throw new InvalidOperationException("Folder does not exist!");
        }

        var hasPathChanged = existingFolder.Name != folder.Name || existingFolder.ParentId != folder.ParentId || existingFolder.TestSuiteId != folder.TestSuiteId;
        if (hasPathChanged)
        {
            await CalculatePathAsync(dbContext, folder);
        }

        dbContext.TestSuiteFolders.Update(folder);
        await dbContext.SaveChangesAsync();

        if(hasPathChanged)
        {
            await UpdateChildTestSuiteFolderPathsAsync(dbContext, folder.Id, folder.TestSuiteId);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task UpdateChildTestSuiteFolderPathsAsync(ApplicationDbContext dbContext, long id, long testSuiteId)
    {
        foreach (var folder in await dbContext.TestSuiteFolders.Where(x => x.ParentId == id).ToListAsync())
        {
            await CalculatePathAsync(dbContext, folder);
            await UpdateChildTestSuiteFolderPathsAsync(dbContext, folder.Id, testSuiteId);
        }

        foreach (var testCase in await dbContext.TestCases.Where(x => x.TestSuiteFolderId == id).ToListAsync())
        {
            testCase.TestSuiteId = testSuiteId;
            await CalculatePathAsync(dbContext, testCase);
        }
    }

    public async Task<TestSuiteFolder?> GetTestSuiteFolderByNameAsync(string tenantId, long testSuiteId, long? parentId, string folderName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var folders = dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.TenantId == tenantId && x.TestSuiteId == testSuiteId && x.ParentId == parentId && x.Name == folderName);
        return await folders.FirstOrDefaultAsync();
    }

    public async Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var folders = dbContext.TestSuiteFolders.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.ParentId == parentFolderId && x.TestSuiteId == testSuiteId)
            .OrderBy(x => x.Name);
        return await folders.ToArrayAsync();
    }

    public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(string tenantId, long? projectId, long testSuiteId, long? parentFolderId, string name)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existingFolder = await dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.TestSuiteId == testSuiteId && x.ParentId == parentFolderId && x.Name == name).FirstOrDefaultAsync();
        if(existingFolder is not null)
        {
            return existingFolder;
        }

        var folder = new TestSuiteFolder
        {
            Name = name,
            TestSuiteId = testSuiteId,
            Created = DateTimeOffset.UtcNow,
            TenantId = tenantId,
            TestProjectId = projectId,
            ParentId = parentFolderId
        };

        await CalculatePathAsync(dbContext, folder);

        await dbContext.TestSuiteFolders.AddAsync(folder);
        await dbContext.SaveChangesAsync();
        return folder;
    }
    #endregion Test Suite Folders

    #region Test Suites

    /// <inheritdoc/>
    public async Task<TestSuite?> GetTestSuiteByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestSuites.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }
    public async Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var suites = dbContext.TestSuites.Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.TeamId is not null)
        {
            suites = suites.Where(x => x.TeamId == query.TeamId);
        }
        if (query.ProjectId is not null)
        {
            suites = suites.Where(x => x.TestProjectId == query.ProjectId);
        }
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
    public async Task UpdateTestSuiteAsync(TestSuite suite)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.TestSuites.Update(suite);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTestSuiteByIdAsync(string tenantId, long testSuiteId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        foreach (var folder in dbContext.TestSuiteFolders.Where(x => x.ParentId == null && x.TestSuiteId == testSuiteId))
        {
            await DeleteFolderByIdAsync(tenantId, folder.Id, dbContext);
        }
        await dbContext.TestSuites.Where(x => x.Id == testSuiteId).ExecuteDeleteAsync();
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

    #endregion Test Suites

    #region Test Suite Folders
    /// <inheritdoc/>
    public async Task DeleteFolderByIdAsync(string tenantId, long folderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await DeleteFolderByIdAsync(tenantId, folderId, dbContext);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
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

    #endregion

    #region Test Runs

    /// <inheritdoc/>
    public async Task<TestRun?> GetTestRunByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestRuns.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task AddTestRunAsync(TestRun testRun)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestRuns.AddAsync(testRun);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<int[]> GetTestRunYearsAsync(string tenantId, long? teamId, long? projectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var runs = dbContext.TestRuns.AsNoTracking().Where(x => x.TenantId == tenantId);
        if (projectId is not null)
        {
            runs = runs.Where(x => x.TestProjectId == projectId);
        }

        var years = runs.Select(x => x.Created.Year).Distinct().ToArray();
        return years;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestRun>> SearchTestRunsAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var runs = dbContext.TestRuns.Where(x => x.TenantId == tenantId);
        var total = await dbContext.TestRuns.LongCountAsync();

        // Apply filter
        if (query.TeamId is not null)
        {
            runs = runs.Where(x => x.TeamId == query.TeamId);
        }
        if (query.ProjectId is not null)
        {
            runs = runs.Where(x => x.TestProjectId == query.ProjectId);
        }
        if (query.Text is not null)
        {
            runs = runs.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await runs.LongCountAsync();
        var items = runs.OrderByDescending(x => x.Created).Skip(query.Offset).Take(query.Count);

        return new PagedResult<TestRun>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }

    public async Task DeleteTestCaseRunByIdAsync(string tenantId, long id)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        //await foreach (var field in dbContext.TestCaseRunFields.Where(x => x.TestCaseRunId == id && x.TenantId == tenantId).AsAsyncEnumerable())
        //{
        //    dbContext.TestCaseRunFields.Remove(field);
        //}
        await foreach (var run in dbContext.TestCaseRuns.Where(x => x.Id == id && x.TenantId == tenantId).AsAsyncEnumerable())
        {
            dbContext.TestCaseRuns.Remove(run);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTestRunByIdAsync(long id)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        //await foreach (var field in dbContext.TestCaseRunFields.Where(x => x.TestRunId == id && x.TenantId == tenantId).AsAsyncEnumerable())
        //{
        //    dbContext.TestCaseRunFields.Remove(field);
        //}

        //await foreach (var field in dbContext.TestRunFields.Where(x => x.TestRunId == id && x.TenantId == tenantId).AsAsyncEnumerable())
        //{
        //    dbContext.TestRunFields.Remove(field);
        //}

        await foreach (var run in dbContext.TestCaseRuns.Where(x => x.TestRunId == id).AsAsyncEnumerable())
        {
            dbContext.TestCaseRuns.Remove(run);
        }

        await foreach (var run in dbContext.TestRuns.Where(x => x.Id == id).AsAsyncEnumerable())
        {
            dbContext.TestRuns.Remove(run);
        }

        await dbContext.SaveChangesAsync();
    }
    #endregion Test Runs

    #region Test Case Runs

    /// <inheritdoc/>
    public async Task<TestCaseRun?> GetTestCaseRunByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCaseRuns.AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(string tenantId, SearchTestQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns.Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.TeamId is not null)
        {
            //tests = tests.Where(x => x.TeamId == query.TeamId);
        }
        if (query.ProjectId is not null)
        {
            tests = tests.Where(x => x.TestProjectId == query.ProjectId);
        }
        if (query.Text is not null)
        {
            tests = tests.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }
        if (query.TestRunId is not null)
        {
            tests = tests.Where(x => x.TestRunId == query.TestRunId);
        }
        if (query.TestSuiteId is not null)
        {
            tests = tests.Where(x => x.TestCase != null && x.TestCase.TestSuiteId == query.TestSuiteId);
        }
        if (query.FolderId is not null)
        {
            tests = tests.Where(x => x.TestCase != null && x.TestCase.TestSuiteFolderId == query.FolderId);
        }

        var allTests = await tests.ToArrayAsync();

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count).ToArray();

        return new PagedResult<TestCaseRun>
        {
            TotalCount = totalCount,
            Items = items,
        };
    }

    public async Task AddTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        testCaseRun.Created = DateTimeOffset.UtcNow;

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestCaseRuns.AddAsync(testCaseRun);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.TestCaseRuns.Update(testCaseRun);
        await dbContext.SaveChangesAsync();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Looks up a project ID from the test suite id for a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
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

    private async Task CalculatePathAsync(ApplicationDbContext dbContext, TestCase testCase)
    {
        List<string> pathComponents = new();
        List<long> pathIds = new();
        await CalculatePathAsync(dbContext, testCase.TestSuiteFolderId, pathComponents, pathIds);
        pathComponents.Reverse();
        pathIds.Reverse();
        testCase.Path = string.Join('/', pathComponents);
        testCase.PathIds = pathIds.ToArray();
    }

    private async Task CalculatePathAsync(ApplicationDbContext dbContext, TestSuiteFolder folder)
    {
        List<string> pathComponents = new();
        List<long> pathIds = new();
        await CalculatePathAsync(dbContext, folder.ParentId, pathComponents, pathIds);
        pathComponents.Reverse();
        pathIds.Reverse();
        folder.Path = string.Join('/', pathComponents);
        folder.PathIds = pathIds.ToArray();
    }

    /// <summary>
    /// Calculates path for a folder and writes it to 
    /// - pathComponents with the folder names
    /// - pathIds with folder IDs
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="folderId"></param>
    /// <param name="pathComponents"></param>
    /// <param name=""></param>
    /// <param name="pathIds"></param>
    /// <returns></returns>
    private async Task CalculatePathAsync(ApplicationDbContext dbContext, long? folderId, List<string> pathComponents, List<long> pathIds)
    {
        while (folderId is not null)
        {
            var folder = await dbContext.TestSuiteFolders.AsNoTracking().Where(x => x.Id == folderId).FirstOrDefaultAsync();
            if (folder is null)
            {
                return;
            }
            pathComponents.Add(folder.Name);
            pathIds.Add(folder.Id);
            folderId = folder.ParentId;
        }
    }

    #endregion Helpers
}
