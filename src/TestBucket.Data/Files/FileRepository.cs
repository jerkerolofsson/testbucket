
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Data.Files;

/// <summary>
/// Contains files/attachments
/// </summary>
public class FileRepository : IFileRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public FileRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddResourceAsync(FileResource resource)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Files.AddAsync(resource);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteResourceByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Files.Where(x => x.Id == id && x.TenantId == tenantId).ExecuteDeleteAsync();
    }

    public async Task<FileResource?> GetResourceByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Files.Where(x => x.Id == id && x.TenantId == tenantId).FirstOrDefaultAsync();
    }


    private static async Task<IReadOnlyList<FileResource>> GetFilesWithoutDataAsync(IQueryable<FileResource> resources)
    {
        var projected = await resources.Select(x => new { x.TenantId, x.Id, x.Name, x.ContentType, x.Length, x.Created }).ToListAsync();
        return projected.Select(x => new FileResource() { ContentType = x.ContentType, Name = x.Name, Id = x.Id, Created = x.Created, Data = [], TenantId = x.TenantId, Length = x.Length }).ToList();
    }

    public async Task<IReadOnlyList<FileResource>> GetRequirementAttachmentsAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.RequirementId == id && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }

    public async Task<IReadOnlyList<FileResource>> GetRequirementSpecificationAttachmentsAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.RequirementSpecificationId == id && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }

    public async Task<IReadOnlyList<FileResource>> GetTestCaseAttachmentsAsync(string tenantId, long testCaseId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.TestCaseId == testCaseId && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }

    public async Task<IReadOnlyList<FileResource>> GetTestCaseRunAttachmentsAsync(string tenantId, long testCaseRunId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.TestCaseRunId == testCaseRunId && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }


    public async Task<IReadOnlyList<FileResource>> GetTestProjectAttachmentsAsync(string tenantId, long testProjectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.TestProjectId == testProjectId && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }

    public async Task<IReadOnlyList<FileResource>> GetTestSuiteAttachmentsAsync(string tenantId, long testSuiteId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.TestSuiteId == testSuiteId && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }

    public async Task<IReadOnlyList<FileResource>> GetTestSuiteFolderAttachmentsAsync(string tenantId, long testSuiteFolderId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var resources = dbContext.Files.Where(x => x.TestSuiteFolderId == testSuiteFolderId && x.TenantId == tenantId).AsQueryable();
        return await GetFilesWithoutDataAsync(resources);
    }
}
