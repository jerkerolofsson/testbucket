using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Data.Files;
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
}
