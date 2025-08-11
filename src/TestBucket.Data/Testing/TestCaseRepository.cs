using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

using Pgvector.EntityFrameworkCore;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.TestSuites.Search;

namespace TestBucket.Data.Testing;

/// <summary>
/// Repository for managing test cases, test suites, test runs, and related entities.
/// Provides methods for CRUD operations, searching, and insights.
/// </summary>
internal class TestCaseRepository : ITestCaseRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ISequenceGenerator _sequenceGenerator;
    public TestCaseRepository(
        IMemoryCache memoryCache,
        IDbContextFactory<ApplicationDbContext> dbContextFactory, ISequenceGenerator sequenceGenerator)
    {
        _memoryCache = memoryCache;
        _dbContextFactory = dbContextFactory;
        _sequenceGenerator = sequenceGenerator;
    }

    #region Test Cases

    /// <summary>
    /// Assigns an external ID to a test case if not already set.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="testCase">The test case to update.</param>
    private async ValueTask AssignExternalIdAsync(ApplicationDbContext dbContext, TestCase testCase)
    {
        // Sequence number only used for internal issues
        if (testCase.ExternalDisplayId is null && testCase.TestProjectId is not null && testCase.TenantId is not null)
        {
            var project = await dbContext.Projects.AsNoTracking().Where(x => x.Id == testCase.TestProjectId).FirstAsync();
            testCase.SequenceNumber = await _sequenceGenerator.GenerateSequenceNumberAsync(testCase.TenantId, testCase.TestProjectId.Value, "testcase", GetMaxSequenceNumber, default);
            testCase.ExternalDisplayId = project.ShortName + "-TC-" + testCase.SequenceNumber;

            testCase.ExternalId ??= testCase.ExternalDisplayId;
        }
    }

    /// <summary>
    /// Gets the maximum sequence number for a test case in a project and tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maximum sequence number.</returns>
    private async ValueTask<int> GetMaxSequenceNumber(string tenantId, long projectId, CancellationToken cancellationToken)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var lastestSequenceNumber = await dbContext.TestCases.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.SequenceNumber != null)
            .OrderByDescending(x => x.SequenceNumber).Select(x => x.SequenceNumber).Take(1).FirstOrDefaultAsync();

        return lastestSequenceNumber ?? 0;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestCase>> SemanticSearchTestCasesAsync(ReadOnlyMemory<float> embeddingVector, int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters)
    {
        var vector = new Pgvector.Vector(embeddingVector);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases
            .AsSingleQuery()
            .Include(x => x.TestCaseFields).AsQueryable();
        foreach (var spec in filters)
        {
            tests = tests.Where(spec.Expression);
        }
        long totalCount = await tests.LongCountAsync();

        tests = tests.Where(x => x.Embedding != null);

        var page = tests.Select(x => new { Test = x, Distance = x.Embedding!.CosineDistance(vector) })
            .Where(x => x.Distance < 1.0)
            .OrderBy(x => x.Distance)
            .Skip(offset)
            .Take(count)
            .Select(x => x.Test);

        return new PagedResult<TestCase>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters, Func<TestCase, object> orderBy, bool descending)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases
            .AsSingleQuery()
            .Include(x => x.TestCaseFields).AsQueryable();
        foreach (var spec in filters)
        {
            tests = tests.Where(spec.Expression);
        }
        long totalCount = await tests.LongCountAsync();

        if (descending)
        {
            return new PagedResult<TestCase>
            {
                TotalCount = totalCount,
                Items = tests.OrderByDescending(orderBy).Skip(offset).Take(count).ToArray(),
            };
        }

        return new PagedResult<TestCase>
        {
            TotalCount = totalCount,
            Items = tests.OrderBy(orderBy).Skip(offset).Take(count).ToArray(),
        };
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TestCase>> SearchTestCasesAsync(int offset, int count, IEnumerable<FilterSpecification<TestCase>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases
            .AsSingleQuery()
            .Include(x => x.TestCaseFields).AsQueryable();
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

    /// <inheritdoc/>
    public async Task<long[]> SearchTestCaseIdsAsync(IEnumerable<FilterSpecification<TestCase>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases.AsNoTracking()
            .AsSingleQuery()
            .Include(x => x.TestCaseFields).AsQueryable();
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
        return await dbContext.TestCases
            .Include(x => x.Comments)
            .AsSingleQuery()
            .AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == testCaseId).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<TestCase?> GetTestCaseBySlugAsync(string tenantId, long? projectId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var query = dbContext.TestCases.AsNoTracking().Where(x => x.TenantId == tenantId && x.Slug == slug);
        if(projectId is not null)
        {
            query = query.Where(x => x.TestProjectId == projectId);
        }
            
        return await query.FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<TestCase?> GetTestCaseByNameAsync(string tenantId, long? projectId, long? testSuiteId, string name)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var query = dbContext.TestCases.AsNoTracking().Where(x => x.TenantId == tenantId && x.Name == name);
        if (projectId is not null)
        {
            query = query.Where(x => x.TestProjectId == projectId);
        }
        if (testSuiteId is not null)
        {
            query = query.Where(x => x.TestSuiteId == testSuiteId);
        }

        return await query.FirstOrDefaultAsync();
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

        if (string.IsNullOrEmpty(testCase.Slug) && testCase.TenantId is not null)
        {
            testCase.Slug = await GenerateTestCaseSlugAsync(testCase.TenantId, testCase.Name);
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tenantId = testCase.TenantId ?? throw new ArgumentException("TenantId missing");
        await AssignExternalIdAsync(dbContext, testCase);

        await LookupProjectIdFromSuiteId(testCase, dbContext);
        await CalculatePathAsync(dbContext, testCase);

        dbContext.TestCases.Add(testCase);
        await dbContext.SaveChangesAsync();

        await GenerateTestSlugIfMissingAsync(testCase, dbContext);
    }

    public async Task DeleteTestCaseByIdAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await DeleteTestCaseByIdAsync(id, dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task DeleteTestCaseByIdAsync(long testCaseId, ApplicationDbContext dbContext)
    {
        // Delete all test case runs
        await DeleteTestCaseRunsByTestCaseIdAsync(testCaseId, dbContext);

        await dbContext.RequirementTestLinks.Where(x => x.TestCaseId == testCaseId).ExecuteDeleteAsync();
        await dbContext.Comments.Where(x => x.TestCaseId == testCaseId).ExecuteDeleteAsync();
        await dbContext.TestCaseFields.Where(x => x.TestCaseId == testCaseId).ExecuteDeleteAsync();

        await dbContext.TestCases
            .Include(x => x.TestSteps)
            .AsSingleQuery()
            .Where(x => x.Id == testCaseId).ExecuteDeleteAsync();
    }

    private static async Task DeleteTestCaseRunsByTestCaseIdAsync(long testCaseId, ApplicationDbContext dbContext)
    {
        foreach (var testCaseRun in dbContext.TestCaseRuns.Where(x => x.TestCaseId == testCaseId))
        {
            await dbContext.Metrics.Where(x => x.TestCaseRunId == testCaseRun.Id).ExecuteDeleteAsync();
            await dbContext.Comments.Where(x => x.TestCaseRunId == testCaseRun.Id).ExecuteDeleteAsync();
            await dbContext.TestCaseRunFields.Where(x => x.TestCaseRunId == testCaseRun.Id).ExecuteDeleteAsync();
        }
        await dbContext.TestCaseRuns.Where(x => x.TestCaseId == testCaseId).ExecuteDeleteAsync();
    }

    public async Task UpdateTestCaseAsync(TestCase testCase)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if (string.IsNullOrEmpty(testCase.Slug) && testCase.TenantId is not null)
        {
            testCase.Slug = await GenerateTestCaseSlugAsync(testCase.TenantId, testCase.Name);
        }
        await CalculatePathAsync(dbContext, testCase);
        await AssignExternalIdAsync(dbContext, testCase);

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

        var hasTestSuiteChanged = existingFolder.TestSuiteId != folder.TestSuiteId;
        if(hasTestSuiteChanged)
        {
            await ChangeFolderTestSuiteAsync(folder.Id, folder.TestSuiteId, dbContext);
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

    private async Task ChangeFolderTestSuiteAsync(long folderId, long testSuiteId, ApplicationDbContext dbContext)
    {
        await foreach (var testCase in dbContext.TestCases.Where(x => x.TestSuiteFolderId == folderId).AsAsyncEnumerable())
        {
            testCase.TestSuiteId = testSuiteId;
            dbContext.TestCases.Update(testCase);
        }
        foreach (var folder in dbContext.TestSuiteFolders.Where(x => x.ParentId == folderId))
        {
            folder.TestSuiteId = testSuiteId;
            dbContext.TestSuiteFolders.Update(folder);

            await ChangeFolderTestSuiteAsync(folder.Id, testSuiteId, dbContext);
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

        var teamId = await dbContext.TestSuites.Where(x => x.Id == testSuiteId).Select(x=>x.TeamId).FirstOrDefaultAsync();

        var folder = new TestSuiteFolder
        {
            Name = name,
            TestSuiteId = testSuiteId,
            Created = DateTimeOffset.UtcNow,
            Modified = DateTimeOffset.UtcNow,
            TenantId = tenantId,
            TeamId = teamId,
            TestProjectId = projectId,
            ParentId = parentFolderId
        };

        await CalculatePathAsync(dbContext, folder);

        dbContext.TestSuiteFolders.Add(folder);
        await dbContext.SaveChangesAsync();
        return folder;
    }

    public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(TestSuiteFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await CalculatePathAsync(dbContext, folder);

        dbContext.TestSuiteFolders.Add(folder);
        await dbContext.SaveChangesAsync();
        return folder;
    }

    #endregion Test Suite Folders

    #region Test Suites

    /// <inheritdoc/>
    public async Task<TestSuite?> GetTestSuiteBySlugAsync(string tenantId, long? projectId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var query = dbContext.TestSuites.AsNoTracking().Where(x => x.TenantId == tenantId && x.Slug == slug);
        if(projectId is not null)
        {
            query = query.Where(x => x.TestProjectId == projectId);
        }
        return await query.FirstOrDefaultAsync();
    }
    /// <inheritdoc/>
    public async Task<TestSuite?> GetTestSuiteByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestSuites
            .Include(x => x.Comments)
            .AsSingleQuery()
            .AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }
    public async Task<PagedResult<TestSuite>> SearchTestSuitesAsync(string tenantId, SearchTestSuiteQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var suites = dbContext.TestSuites
            .Include(x => x.TestProject)
            .AsSingleQuery()
            .Where(x => x.TenantId == tenantId);

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
        if (query.RootFolders == true)
        {
            suites = suites.Where(x => x.FolderId == null);
        }
        if (query.FolderId is not null)
        {
            suites = suites.Where(x => x.FolderId == query.FolderId);
        }

        long totalCount = await suites.LongCountAsync();
        var items = suites.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

        // Some old suite didnt have team:
        //foreach(var item in items)
        //{
        //    if(item.TeamId is null)
        //    {
        //        item.TeamId = item.TestProject?.TeamId;
        //        dbContext.TestSuites.Update(item);
        //        await dbContext.SaveChangesAsync();
        //    }
        //}

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
    public async Task UpdateTestSuiteAsync(TestSuite testSuite)
    {
        // Generate a slug
        if (string.IsNullOrEmpty(testSuite.Slug) && testSuite.TenantId is not null)
        {
            testSuite.Slug = await GenerateTestSuiteSlugAsync(testSuite.TenantId, testSuite.Name);
        }

        //testCase.Created = DateTimeOffset.UtcNow;
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.TestSuites.Update(testSuite);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTestSuiteByIdAsync(string tenantId, long testSuiteId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Delete all test cases
        foreach (var testCase in dbContext.TestCases.Where(x => x.TestSuiteId == testSuiteId))
        {
            await DeleteTestCaseByIdAsync(testCase.Id, dbContext);
        }
        await dbContext.SaveChangesAsync();

        // Delete all root test suites
        foreach (var folder in dbContext.TestSuiteFolders.Where(x => x.TestSuiteId == testSuiteId && x.ParentId == null))
        {
            await DeleteFolderByIdAsync(folder.Id, dbContext, recurse: true);
        }

        // Delete comments
        foreach (var comment in dbContext.Comments.Where(x => x.TestSuiteId == testSuiteId))
        {
            dbContext.Comments.Remove(comment);
        }

        // Delete any dangling folders
        foreach (var folder in dbContext.TestSuiteFolders.Where(x => x.TestSuiteId == testSuiteId))
        {
            await DeleteFolderByIdAsync(folder.Id, dbContext, recurse: true);
        }
        await dbContext.SaveChangesAsync();
        await dbContext.TestSuites.Where(x => x.Id == testSuiteId).ExecuteDeleteAsync();
    }
    private async Task<bool> TestCaseRunSlugExistsAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCaseRuns.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
    }
    private async Task<bool> TestCaseSlugExistsAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCases.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
    }
    private async Task<bool> TestSuiteSlugExistsAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestSuites.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
    }

    /// <summary>
    /// Returns true if a project exists with the specified slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<bool> TestRunSlugExistsAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestRuns.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
    }

    /// <summary>
    /// Generates a unique slug
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<string> GenerateTestCaseRunSlugAsync(string tenantId, string name)
    {
        var slugHelper = new Slugify.SlugHelper();
        var slug = slugHelper.GenerateSlug(name);
        int counter = 1;
        while (await TestCaseRunSlugExistsAsync(tenantId, slug))
        {
            slug = slugHelper.GenerateSlug(slug + counter);
            counter++;
        }
        return slug;
    }
    /// <summary>
    /// Generates a unique slug
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<string> GenerateTestRunSlugAsync(string tenantId, string name)
    {
        var slugHelper = new Slugify.SlugHelper();
        var slug = slugHelper.GenerateSlug(name);
        int counter = 1;
        while (await TestRunSlugExistsAsync(tenantId, slug))
        {
            slug = slugHelper.GenerateSlug(slug + counter);
            counter++;
        }
        return slug;
    }
    private async Task<string> GenerateTestCaseSlugAsync(string tenantId, string name)
    {
        var slugHelper = new Slugify.SlugHelper();
        var slug = slugHelper.GenerateSlug(name);
        int counter = 1;
        while (await TestCaseSlugExistsAsync(tenantId, slug))
        {
            slug = slugHelper.GenerateSlug(slug + counter);
            counter++;
        }
        return slug;
    }
    private async Task<string> GenerateTestSuiteSlugAsync(string tenantId, string name)
    {
        var slugHelper = new Slugify.SlugHelper();
        var slug = slugHelper.GenerateSlug(name);
        int counter = 1;
        while (await TestSuiteSlugExistsAsync(tenantId, slug))
        {
            slug = slugHelper.GenerateSlug(slug + counter);
            counter++;
        }
        return slug;
    }


    public async Task<TestSuite> AddTestSuiteAsync(TestSuite testSuite)
    {
        ArgumentNullException.ThrowIfNull(testSuite.TenantId);
        ArgumentNullException.ThrowIfNull(testSuite.TeamId);
        ArgumentNullException.ThrowIfNull(testSuite.TestProjectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(testSuite.Name);
        // Generate a slug
        if (string.IsNullOrEmpty(testSuite.Slug))
        {
            testSuite.Slug = await GenerateTestSuiteSlugAsync(testSuite.TenantId, testSuite.Name);
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        try
        {
            dbContext.TestSuites.Add(testSuite);
            await dbContext.SaveChangesAsync();
            return testSuite;
        }
        catch(Exception ex)
        {
            var teamExists = await dbContext.Teams.Where(x => x.TenantId == testSuite.TenantId && x.Id == testSuite.TeamId).AnyAsync();
            var projectExists = await dbContext.Projects.Where(x => x.TenantId == testSuite.TenantId && x.Id == testSuite.TestProjectId).AnyAsync();

            throw new Exception($"An errror occured when adding test suite: slug='{testSuite.Slug}', team='{testSuite.TeamId}' ({teamExists}), project='{testSuite.TestProjectId}' ({projectExists})", ex);
        }
    }

    #endregion Test Suites

    #region Test Suite Folders
    /// <inheritdoc/>
    public async Task DeleteFolderByIdAsync(string tenantId, long folderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await DeleteFolderByIdAsync(folderId, dbContext, recurse: true);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    private async Task DeleteFolderByIdAsync(long folderId, ApplicationDbContext dbContext, bool recurse)
    {
        if (recurse)
        {
            foreach (var childFolder in dbContext.TestSuiteFolders.Where(x => x.ParentId == folderId))
            {
                if (childFolder.Id != folderId)
                {
                    await DeleteFolderByIdAsync(childFolder.Id, dbContext, recurse: true);
                }
            }
        }

        foreach (var testCase in dbContext.TestCases.Where(x=>x.TestSuiteFolderId == folderId))
        {
            await DeleteTestCaseByIdAsync(testCase.Id, dbContext);
        }

        await dbContext.TestSuiteFolders.Where(x =>  x.Id == folderId).ExecuteDeleteAsync();
    }

    #endregion

    #region Test Runs

    /// <inheritdoc/>
    public async Task<TestRun?> GetTestRunBySlugAsync(string tenantId, long? projectId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var query = dbContext.TestRuns.AsNoTracking()
            .Include(x => x.TestRunFields)
            .Include(x => x.Team)
            .Include(x => x.TestProject)
            .Include(x => x.Comments)
            .AsSplitQuery()
            .Where(x => x.TenantId == tenantId && x.Slug == slug);

        if(projectId is not null)
        {
            query = query.Where(x => x.TestProjectId == projectId);
        }

        return await query.FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<TestRun?> GetTestRunByIdAsync(string tenantId, long id)
    {
        string key = "run:" + tenantId + id;

        return await _memoryCache.GetOrCreateAsync(key, async (e) =>
        {
            e.SetSlidingExpiration(TimeSpan.FromMinutes(2));

            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.TestRuns.AsNoTracking()
                .Include(x => x.TestRunFields)
                .Include(x => x.Team)
                .Include(x => x.TestProject)
                .Include(x => x.Comments)
                .AsSplitQuery()
                .Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
        });
    }

    /// <inheritdoc/>
    public async Task UpdateTestRunAsync(TestRun testRun)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        string key = "run:" + testRun.TenantId + testRun.Id;
        _memoryCache.Remove(key);

        dbContext.TestRuns.Update(testRun);
        await dbContext.SaveChangesAsync();
    }


    /// <inheritdoc/>
    public async Task AddTestRunAsync(TestRun testRun)
    {
        ArgumentNullException.ThrowIfNull(testRun.TenantId);

        testRun.Slug = await GenerateTestCaseSlugAsync(testRun.TenantId, testRun.Name);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if (testRun.TestProjectId is not null && testRun.TeamId is null)
        {
            var project = await dbContext.Projects.Where(x => x.Id == testRun.TestProjectId).FirstOrDefaultAsync();
            testRun.TeamId = project?.TeamId;   
        }

        dbContext.TestRuns.Add(testRun);
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
        var runs = dbContext.TestRuns
            .Include(x=>x.TestRunFields!).ThenInclude(u=>u.FieldDefinition)
            .AsSplitQuery()
            .AsQueryable();

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
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await foreach (var metric in dbContext.Metrics.Where(x => x.Id == id && x.TenantId == tenantId).AsAsyncEnumerable())
        {
            dbContext.Metrics.Remove(metric);
        }
        await foreach (var linkedIssue in dbContext.LinkedIssues.Where(x => x.Id == id && x.TenantId == tenantId).AsAsyncEnumerable())
        {
            dbContext.LinkedIssues.Remove(linkedIssue);
        }
        await foreach (var run in dbContext.TestCaseRuns.Where(x => x.Id == id && x.TenantId == tenantId).AsAsyncEnumerable())
        {
            dbContext.TestCaseRuns.Remove(run);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTestRunAsync(TestRun testRun)
    {
        var id = testRun.Id;

        string key = "run:" + testRun.TenantId + testRun.Id;
        _memoryCache.Remove(key);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await foreach (var job in dbContext.Jobs.Where(x => x.TestRunId == id).AsAsyncEnumerable())
        {
            dbContext.Jobs.Remove(job);
        }

        await foreach (var run in dbContext.TestCaseRuns
            .Include(x => x.TestCaseRunFields)
            .Include(x => x.LinkedIssues)
            .Include(x => x.Comments)
            .Include(x => x.Metrics)
            .AsSplitQuery()
            .Where(x => x.TestRunId == id).AsAsyncEnumerable())
        {
            dbContext.TestCaseRuns.Remove(run);
        }

        await foreach (var run in dbContext.TestRuns.Where(x => x.Id == id).AsAsyncEnumerable())
        {
            dbContext.TestRuns.Remove(run);
        }

        await foreach (var pipelineJob in dbContext.PipelineJobs.Where(x => x.TestRunId == id).AsAsyncEnumerable())
        {
            dbContext.PipelineJobs.Remove(pipelineJob);
        }

        await foreach (var pipeline in dbContext.Pipelines.Where(x => x.TestRunId == id).AsAsyncEnumerable())
        {
            dbContext.Pipelines.Remove(pipeline);
        }

        await dbContext.SaveChangesAsync();
    }
    #endregion Test Runs

    #region Test Case Runs

    /// <inheritdoc/>
    public async Task<TestCaseRun?> GetTestCaseRunByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestCaseRuns.AsNoTracking().Include(x => x.LinkedIssues).AsNoTracking()
            .Include(x => x.TestCase)
            .Include(x => x.Comments)
            .Include(x => x.Metrics)
            .AsSplitQuery()
            .Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(IReadOnlyList<FilterSpecification<TestCaseRun>> filters, int offset,int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .AsNoTracking()
            .Include(x => x.LinkedIssues)
            .Include(x => x.Comments)
            .Include(x => x.TestCase)
            .Include(x => x.Metrics)
            .Include(x => x.TestCaseRunFields!).ThenInclude(y => y.FieldDefinition)
            .AsSplitQuery()
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
    public async Task DeleteTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestCaseRuns.Remove(testCaseRun);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddTestCaseRunAsync(TestCaseRun testCaseRun)
    {
        ArgumentNullException.ThrowIfNull(testCaseRun.TenantId);
        if(testCaseRun.Slug is null)
        {
            var testRun = await GetTestRunByIdAsync(testCaseRun.TenantId, testCaseRun.TestRunId);
            var testCase = await GetTestCaseByIdAsync(testCaseRun.TenantId, testCaseRun.TestCaseId);
            if (testRun is null)
            {
                throw new ArgumentException("Test run not found for this tenant, id=" + testCaseRun.TestRunId);
            }
            if (testCase is null)
            {
                throw new ArgumentException("Test case not found for this tenant, id=" + testCaseRun.TestCaseId);
            }
            testCaseRun.Slug = await GenerateTestCaseRunSlugAsync(testCaseRun.TenantId, testRun.Slug + "_" + testCase.Slug);
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestCaseRuns.Add(testCaseRun);
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
            .AsSplitQuery()
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
            result.Failed = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Failed).Sum(x => x.Count);
            result.Blocked = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Blocked).Sum(x => x.Count);
            result.Skipped = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Skipped).Sum(x => x.Count);
            result.Error = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Error).Sum(x => x.Count);
            result.Assert = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Assert).Sum(x => x.Count);
            result.Hang = allResults.Where(x => x.FieldValue == fieldValue && x.Result == TestResult.Hang).Sum(x => x.Count);
            table[name] = result;
        }

        return table;
    }

    public async Task<Dictionary<long, TestExecutionResultSummary>> GetTestExecutionResultSummaryForRunsAsync(
        IReadOnlyList<long> testRunsIds,
        IEnumerable<FilterSpecification<TestCaseRun>> filters)
    {
        var table = new Dictionary<long, TestExecutionResultSummary>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Where(x=> testRunsIds.Contains(x.TestRunId))
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var allResults = await tests.GroupBy(x => new { x.Result, x.TestRunId }).
            Select(g => new { Count = g.Count(), g.Key.Result, g.Key.TestRunId }).ToListAsync();

        foreach (var fieldValue in allResults.Select(x => x.TestRunId).Distinct())
        {
            var name = fieldValue;

            var result = new TestExecutionResultSummary();
            result.Total = allResults.Where(x => x.TestRunId == fieldValue).Sum(x => x.Count);
            result.Completed = allResults.Where(x => x.TestRunId == fieldValue && x.Result != TestResult.NoRun).Sum(x => x.Count);
            result.Passed = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Passed).Sum(x => x.Count);
            result.Failed = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Failed).Sum(x => x.Count);
            result.Blocked = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Blocked).Sum(x => x.Count);
            result.Skipped = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Skipped).Sum(x => x.Count);
            result.Error = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Error).Sum(x => x.Count);
            result.Assert = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Assert).Sum(x => x.Count);
            result.Hang = allResults.Where(x => x.TestRunId == fieldValue && x.Result == TestResult.Hang).Sum(x => x.Count);
            table[name] = result;
        }

        return table;
    }

    public async Task<Dictionary<DateOnly, TestExecutionResultSummary>> GetTestExecutionResultSummaryByDayAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters)
    {
        var table = new Dictionary<DateOnly, TestExecutionResultSummary>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        tests = tests.Where(x=>x.Modified > new DateTimeOffset(1980,1,1,0,0,0,TimeSpan.Zero));

        var allResults = await tests.GroupBy(x => new { x.Result, x.Modified.Year, x.Modified.Month, x.Modified.Day }).
            Select(g => new { Count = g.Count(), g.Key.Result, g.Key.Year, g.Key.Month, g.Key.Day }).ToListAsync();

        foreach (var fieldValue in allResults.Select(x => new { x.Year, x.Month, x.Day }).Distinct())
        {
            if (fieldValue is not null)
            {
                var result = new TestExecutionResultSummary();
                result.Total = allResults.Where(x =>     x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day).Sum(x => x.Count);
                result.Completed = allResults.Where(x => x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result != TestResult.NoRun).Sum(x => x.Count);
                result.Passed = allResults.Where(x =>    x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Passed).Sum(x => x.Count);
                result.Failed = allResults.Where(x =>    x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Failed).Sum(x => x.Count);
                result.Blocked = allResults.Where(x =>   x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Blocked).Sum(x => x.Count);
                result.Skipped = allResults.Where(x =>   x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Skipped).Sum(x => x.Count);
                result.Error = allResults.Where(x =>     x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Error).Sum(x => x.Count);
                result.Assert = allResults.Where(x =>    x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Assert).Sum(x => x.Count);
                result.Hang = allResults.Where(x =>      x.Year == fieldValue.Year && x.Month == fieldValue.Month && x.Day == fieldValue.Day && x.Result == TestResult.Hang).Sum(x => x.Count);

                var date = new DateOnly(fieldValue.Year, fieldValue.Month, fieldValue.Day);
                table[date] = result;
            }
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
            .AsSplitQuery()
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

    /// <summary>
    /// Returns number of testcases grouped per field
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="fieldDefinitionId"></param>
    /// <returns></returns>
    public async Task<InsightsData<string, int>> GetInsightsTestCountPerFieldAsync(IEnumerable<FilterSpecification<TestCase>> filters, long fieldDefinitionId)
    {
        var data = new InsightsData<string, int>() { Name = "test-count" };
        var series = new InsightsSeries<string, int>() { Name = "test-by-field" };
        data.Add(series);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var localIsses = dbContext.TestCases
            .Include(x => x.TestCaseFields)
            .AsSingleQuery()
            .Where(x => x.TestCaseFields!.Any(f => f.FieldDefinitionId == fieldDefinitionId))
            .AsQueryable();

        foreach (var filter in filters)
        {
            localIsses = localIsses.Where(filter.Expression);
        }

        var issues = await localIsses.GroupBy(x => new { x.TestCaseFields!.First(x => x.FieldDefinitionId == fieldDefinitionId).StringValue }).
            Select(g => new { FieldValue = g.Key.StringValue, Count = g.Count() }).ToListAsync();

        foreach (var item in issues)
        {
            var name = item.FieldValue ?? "(null)";
            series.Add(name, item.Count);
        }

        return data;
    }

    /// <inheritdoc/>
    public async Task<InsightsData<DateOnly, int>> GetInsightsTestResultsByDayAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters)
    {
        var data = new InsightsData<DateOnly, int>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        tests = tests.Where(x => x.Modified > new DateTimeOffset(1980, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var allResults = await tests.GroupBy(x => new { x.Result, x.Modified.Year, x.Modified.Month, x.Modified.Day }).
            Select(g => new { Count = g.Count(), g.Key.Result, g.Key.Year, g.Key.Month, g.Key.Day }).ToListAsync();

        foreach(var result in new TestResult[] { TestResult.Passed, TestResult.Failed })
        { 
            var series = data.Add(result.ToString());
            foreach (var dayItem in allResults.Where(x=>x.Result == result).Select(x => new { x.Year, x.Month, x.Day, x.Count }))
            {
                var date = new DateOnly(dayItem.Year, dayItem.Month, dayItem.Day);
                series.Add(date, dayItem.Count);
            }
        }

        return data;
    }

    /// <summary>
    /// Returns one series per field.StringValue
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="fieldDefinitionId"></param>
    /// <returns></returns>
    public async Task<InsightsData<string, int>> GetInsightsTestResultsByFieldAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters, long fieldDefinitionId)
    {
        var data = new InsightsData<string, int>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .Where(x => x.TestCaseRunFields!.Any(f => f.FieldDefinitionId == fieldDefinitionId))
            .AsQueryable();

        foreach (var filter in filters)
        {
            var count1 = await tests.CountAsync();

            tests = tests.Where(filter.Expression);

            var count2 = await tests.CountAsync();
        }

        var results = await tests.GroupBy(x => new { x.TestCaseRunFields!.First(x => x.FieldDefinitionId == fieldDefinitionId).StringValue, x.Result })
            .Select(g => new { Count = g.Count(), Result = g.Key.Result, FieldValue = g.Key.StringValue })
            .ToListAsync();

        Dictionary<string, InsightsSeries<string, int>> series = [];
        foreach (var row in results)
        {
            var seriesName = row.FieldValue ?? "(null)";
            if(!series.TryGetValue(seriesName, out var s))
            {
                s = new InsightsSeries<string, int>() { Name = seriesName };
                series[seriesName] = s;
                data.Add(s);
            }
            s.Add(row.Result.ToString(), row.Count);
        }

        return data;
    }

    /// <inheritdoc/>
    public async Task<InsightsData<string, int>> GetInsightsTestCaseRunCountByAssigneeAsync(List<FilterSpecification<TestCaseRun>> filters)
    {
        var data = new InsightsData<string, int>();
        var series = data.Add("results");

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var results = await tests.GroupBy(x => x.AssignedToUserName).Select(g => new { Count = g.Count(), AssignedToUserName = g.Key }).ToListAsync();
        foreach (var row in results)
        {
            series.Add(row.AssignedToUserName ?? "(null)", row.Count);
        }

        return data;
    }

    /// <inheritdoc/>
    public async Task<InsightsData<TestResult, int>> GetInsightsTestResultsAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters)
    {
        var data = new InsightsData<TestResult, int>();
        var series = data.Add("results");

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var results = await tests.GroupBy(x => x.Result).Select(g => new { Count = g.Count(), Result = g.Key }).ToListAsync();
        foreach(var row in results)
        {
            series.Add(row.Result, row.Count);
        }
       
        return data;
    }

    /// <inheritdoc/>
    public async Task<InsightsData<TestResult, int>> GetInsightsLatestTestResultsAsync(IEnumerable<FilterSpecification<TestCaseRun>> filters)
    {
        var data = new InsightsData<TestResult, int>();
        var series = data.Add("results");

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCaseRuns
            .Include(x => x.TestCase)
            .Include(x => x.TestCaseRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var latest = await tests.GroupBy(x => x.TestCaseId)
            .Select(g => g.OrderByDescending(x => x.Modified).First())
            .ToListAsync();

        var results = latest.GroupBy(x => x.Result).Select(g => new { Count = g.Count(), Result = g.Key }).ToList();
        foreach (var row in results)
        {
            series.Add(row.Result, row.Count);
        }

        return data;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, Dictionary<string, long>>> GetTestCaseCoverageMatrixByFieldAsync(List<FilterSpecification<TestCase>> filters, long fieldDefinitionId1, long fieldDefinitionId2)
    {
        var table = new Dictionary<string, Dictionary<string, long>>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases
            .Include(x => x.TestCaseFields)
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var allResults = await tests.GroupBy(x => new { f1 = x.TestCaseFields!.First(x => x.FieldDefinitionId == fieldDefinitionId1).StringValue, f2 = x.TestCaseFields!.First(x => x.FieldDefinitionId == fieldDefinitionId2).StringValue }).
            Select(g => new { Count = g.Count(), f1 = g.Key.f1, f2 = g.Key.f2 }).ToListAsync();

        foreach (var row in allResults)
        {

            var f1 = row.f1 ?? "(null)";
            var f2 = row.f2 ?? "(null)";
            if (!table.ContainsKey(f1))
            {
                table[f1] = new Dictionary<string, long>();
            }
            table[f1][f2] = row.Count;
        }

        return table;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> GetTestCaseDistributionByFieldAsync(List<FilterSpecification<TestCase>> filters, long fieldDefinitionId)
    {
        var table = new Dictionary<string, long>();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.TestCases
            .Include(x => x.TestCaseFields)
            .AsSingleQuery()
            .Where(x => x.TestCaseFields!.Any(f => f.FieldDefinitionId == fieldDefinitionId))
            .AsQueryable();

        foreach (var filter in filters)
        {
            tests = tests.Where(filter.Expression);
        }

        var allResults = await tests.GroupBy(x => new { x.TestCaseFields!.First(x => x.FieldDefinitionId == fieldDefinitionId).StringValue }).
            Select(g => new { Count = g.Count(), FieldValue = g.Key.StringValue }).ToListAsync();

        foreach (var row in allResults)
        {
            var name = row.FieldValue ?? "(null)";
            table[name] = row.Count;
        }

        return table;
    }

    /// <inheritdoc/>
    public async Task AddTestRepositoryFolderAsync(TestRepositoryFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestRepositoryFolders.Add(folder);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task AddTestLabFolderAsync(TestLabFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestLabFolders.Add(folder);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TestRepositoryFolder>> GetRootTestRepositoryFoldersAsync(long projectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestRepositoryFolders
            .AsNoTracking()
            .Where(x => x.ParentId == null && x.TestProjectId == projectId).ToListAsync();
        
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TestLabFolder>> GetRootTestLabFoldersAsync(long projectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestLabFolders
            .AsNoTracking()
            .Where(x => x.ParentId == null && x.TestProjectId == projectId).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TestRepositoryFolder>> GetChildTestRepositoryFoldersAsync(long projectId, long parentId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestRepositoryFolders
            .AsNoTracking()
            .Where(x => x.ParentId == parentId && x.TestProjectId == projectId).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TestLabFolder>> GetChildTestLabFoldersAsync(long projectId, long parentId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestLabFolders
            .AsNoTracking()
            .Where(x => x.ParentId == parentId && x.TestProjectId == projectId).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateTestRepositoryFolderAsync(TestRepositoryFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestRepositoryFolders.Update(folder);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateTestLabFolderAsync(TestLabFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.TestLabFolders.Update(folder);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteTestRepositoryFolderAsync(TestRepositoryFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await DeleteTestRepositoryFolderAsync(folder, dbContext);
    }

    private async Task DeleteTestRepositoryFolderAsync(TestRepositoryFolder folder, ApplicationDbContext dbContext)
    {
        // Delete all child folders
        foreach (var childFolder in dbContext.TestRepositoryFolders.Where(x => x.ParentId == folder.Id))
        {
            if (childFolder.Id != folder.Id)
            {
                await DeleteTestRepositoryFolderAsync(childFolder, dbContext);
            }
        }

        // Delete any test suites
        foreach (var testSuite in dbContext.TestSuites.Where(x => x.FolderId == folder.Id))
        {
            if (testSuite.TenantId is not null)
            {
                await DeleteTestSuiteByIdAsync(testSuite.TenantId, testSuite.Id);
            }
            else
            {
                testSuite.FolderId = null;
                await UpdateTestSuiteAsync(testSuite);
            }
        }

        // Delete the folder
        await dbContext.TestRepositoryFolders.Where(x => x.Id == folder.Id).ExecuteDeleteAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteTestLabFolderAsync(TestLabFolder folder)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await DeleteTestLabFolderAsync(folder, dbContext);
    }

    private async Task DeleteTestLabFolderAsync(TestLabFolder folder, ApplicationDbContext dbContext)
    {
        // Delete all child folders
        foreach (var childFolder in dbContext.TestLabFolders.Where(x => x.ParentId == folder.Id))
        {
            if (childFolder.Id != folder.Id)
            {
                await DeleteTestLabFolderAsync(childFolder, dbContext);
            }
        }

        // Delete any test runs
        foreach (var testRun in dbContext.TestRuns.Where(x => x.FolderId == folder.Id))
        {
            await DeleteTestRunAsync(testRun);
        }

        // Delete the folder
        await dbContext.TestLabFolders.Where(x => x.Id == folder.Id).ExecuteDeleteAsync();
    }

    /// <inheritdoc/>
    public async Task<TestRepositoryFolder?> GetTestRepositoryFolderByIdAsync(long folderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.TestRepositoryFolders
            .AsNoTracking()
            .Where(x => x.Id == folderId).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<TestLabFolder?> GetTestLabFolderByIdAsync(long folderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.TestLabFolders
            .AsNoTracking()
            .Where(x => x.Id == folderId).FirstOrDefaultAsync();
    }
}
