using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;

using UglyToad.PdfPig.Filters;

namespace TestBucket.Data.Testing;
internal class TestCaseRepository : ITestCaseRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestCaseRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region Test Cases

    /// <inheritdoc/>

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
        ArgumentNullException.ThrowIfNull(testCase.TestProjectId);
        ArgumentNullException.ThrowIfNull(testCase.TeamId);

        testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tenantId = testCase.TenantId ?? throw new ArgumentException("TenantId missing");

        await LookupProjectIdFromSuiteId(testCase, dbContext);
        await CalculatePathAsync(dbContext, testCase);

        await dbContext.TestCases.AddAsync(testCase);
        await dbContext.SaveChangesAsync();

        await GenerateTestSlugIfMissingAsync(testCase, dbContext);
    }

    public async Task DeleteTestCaseByIdAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await RemoveTestCaseByIdAsync(id, dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task RemoveTestCaseByIdAsync(long id, ApplicationDbContext dbContext)
    {
        await foreach (var link in dbContext.RequirementTestLinks.Where(x => x.TestCaseId == id).AsAsyncEnumerable())
        {
            dbContext.RequirementTestLinks.Remove(link);
        }

        foreach (var testCaseRun in await dbContext.TestCaseRuns.Where(x => x.TestCaseId == id).ToListAsync())
        {
            foreach (var field in await dbContext.TestCaseRunFields.Where(x => x.TestCaseRunId == testCaseRun.Id).ToListAsync())
            {
                dbContext.TestCaseRunFields.Remove(field);
            }

            dbContext.TestCaseRuns.Remove(testCaseRun);
        }

        await foreach (var field in dbContext.TestCaseFields.Where(x => x.TestCaseId == id).AsAsyncEnumerable())
        {
            dbContext.TestCaseFields.Remove(field);
        }

        await foreach (var test in dbContext.TestCases.Where(x => x.Id == id).AsAsyncEnumerable())
        {
            dbContext.TestCases.Remove(test);
        }
    }

    public async Task UpdateTestCaseAsync(TestCase testCase)
    {
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

        var testSuite = await dbContext.TestSuites.Where(x => x.Id == testSuiteId).FirstOrDefaultAsync();
        long? teamId = testSuite?.TeamId;

        var folder = new TestSuiteFolder
        {
            Name = name,
            TestSuiteId = testSuiteId,
            Created = DateTimeOffset.UtcNow,
            TenantId = tenantId,
            TeamId = teamId,
            TestProjectId = projectId,
            ParentId = parentFolderId
        };

        await CalculatePathAsync(dbContext, folder);

        await dbContext.TestSuiteFolders.AddAsync(folder);
        await dbContext.SaveChangesAsync();
        return folder;
    }

    public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

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
        await dbContext.SaveChangesAsync();
        await dbContext.TestSuites.Where(x => x.Id == testSuiteId).ExecuteDeleteAsync();
    }

    public async Task<TestSuite> AddTestSuiteAsync(TestSuite testSuite)
    {
       
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

        foreach (var testCase in await dbContext.TestCases.Where(x=>x.TestSuiteFolderId == folderId).ToListAsync())
        {
            await RemoveTestCaseByIdAsync(testCase.Id, dbContext);
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
    public async Task UpdateTestRunAsync(TestRun testRun)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.TestRuns.Update(testRun);
        await dbContext.SaveChangesAsync();
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
    public async Task<PagedResult<TestRun>> SearchTestRunsAsync(IReadOnlyList<FilterSpecification<TestRun>> filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var runs = dbContext.TestRuns.AsQueryable();

        foreach(var filter in filters)
        {
            runs = runs.Where(filter.Expression);
        }

        long totalCount = await runs.LongCountAsync();
        var items = runs.OrderByDescending(x => x.Created).Skip(offset).Take(count);

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
        return await dbContext.TestCaseRuns.AsNoTracking()
            .Include(x=>x.TestCase)
            .Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(IReadOnlyList<FilterSpecification<TestCaseRun>> filters, int offset,int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsQueryable();

        foreach(var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Name).Skip(offset).Take(count).ToArray();

        return new PagedResult<TestCaseRun>
        {
            TotalCount = totalCount,
            Items = items,
        };
    }

    public async Task AddTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.TestCaseRuns.AddAsync(testCaseRun);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateTestCaseRunAsync(TestCaseRun testCaseRun)
    {
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

    public async Task<Dictionary<string,TestExecutionResultSummary>> GetTestExecutionResultSummaryByFieldAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters, long fieldDefinitionId)
    {
        var table = new Dictionary<string, TestExecutionResultSummary>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .Where(x=>x.TestCaseRunFields!.Any(f => f.FieldDefinitionId == fieldDefinitionId))
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var allResults = await tests.GroupBy(x => new { x.Result, x.TestCaseRunFields!.First(x => x.FieldDefinitionId == fieldDefinitionId).StringValue }).
            Select(g => new { Count = g.Count(),g.Key.Result, FieldValue = g.Key.StringValue }).ToListAsync();

        foreach(var fieldValue in allResults.Select(x=>x.FieldValue).Distinct())
        {
            var name = fieldValue ?? "(null)";

            var result = new TestExecutionResultSummary();
            result.Total = allResults.Where(x=>x.FieldValue == fieldValue).Sum(x => x.Count);
            result.Completed = allResults.Where(x => x.FieldValue == fieldValue && x.Result != TestResult.NoRun).Sum(x => x.Count);
            result.Passed = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Passed).Sum(x => x.Count);
            result.Failed = allResults.Where(x => x.FieldValue == fieldValue && x.FieldValue == fieldValue && x.Result == TestResult.Failed).Sum(x => x.Count);
            result.Blocked = allResults.Where(x => x.Result == TestResult.Blocked).Sum(x => x.Count);
            result.Skipped = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Skipped).Sum(x => x.Count);
            result.Error = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Error).Sum(x => x.Count);
            result.Assert = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Assert).Sum(x => x.Count);
            result.Hang = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Hang).Sum(x => x.Count);
            table[name] = result;
        }

        return table;
    }

    public async Task<TestExecutionResultSummary> GetTestExecutionResultSummaryAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters)
    {
        var result = new TestExecutionResultSummary();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var results = await tests.GroupBy(x => x.Result).Select(g => new { Count = g.Count(), Result = g.Key }).ToListAsync();
        result.Total        = results.Sum(x=>x.Count);
        result.Completed    = results.Where(x => x.Result != TestResult.NoRun   ).Sum(x => x.Count);
        result.Passed       = results.Where(x => x.Result == TestResult.Passed  ).Sum(x => x.Count);
        result.Failed       = results.Where(x => x.Result == TestResult.Failed  ).Sum(x => x.Count);
        result.Blocked      = results.Where(x => x.Result == TestResult.Blocked ).Sum(x => x.Count);
        result.Skipped      = results.Where(x => x.Result == TestResult.Skipped ).Sum(x => x.Count);
        result.Error        = results.Where(x => x.Result == TestResult.Error   ).Sum(x => x.Count);
        result.Assert       = results.Where(x => x.Result == TestResult.Assert  ).Sum(x => x.Count);
        result.Hang         = results.Where(x => x.Result == TestResult.Hang    ).Sum(x => x.Count);

        return result;
    }


}
