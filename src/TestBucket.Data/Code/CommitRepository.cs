using System;
using System.Linq;

using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Data.Code;
public class CommitRepository : ICommitRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public CommitRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task DeleteCommitByShaAsync(string sha)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach(var commit in dbContext.Commits
            .Include(x => x.CommitFiles)
            .Where(x => x.Sha == sha))
        {
            dbContext.Commits.Remove(commit);
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCommitAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach (var commit in dbContext.Commits
            .Include(x=>x.CommitFiles)
            .Where(x => x.Id == id))
        {
            dbContext.Commits.Remove(commit);
        }
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateCommitAsync(Commit value)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Commits.Update(value);
        await dbContext.SaveChangesAsync();
    }
    public async Task AddCommitAsync(Commit value)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Commits.AddAsync(value);
        await dbContext.SaveChangesAsync();

    }

    public async Task AddRepositoryAsync(Repository repo)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Repositories.AddAsync(repo);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Commit?> GetCommitByShaAsync(string tenantId, string sha)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Commits.AsNoTracking().Where(x => x.TenantId == tenantId && x.Sha == sha).FirstOrDefaultAsync();
    }

    public async Task<Repository?> GetRepoByExternalSystemAsync(long externalSystemId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Repositories.AsNoTracking().Where(x => x.ExternalSystemId == externalSystemId).FirstOrDefaultAsync();
    }

    public async Task<Repository?> GetRepoByUrlAsync(string tenantId, string url)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Repositories.AsNoTracking().Where(x => x.Url == url).FirstOrDefaultAsync();

    }

    public async Task<PagedResult<Commit>> SearchCommitsAsync(FilterSpecification<Commit>[] filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var commits = dbContext.Commits
            .Include(x => x.CommitFiles)
            .AsNoTracking();
        foreach (var filter in filters)
        {
            commits = commits.Where(filter.Expression);
        }
        long totalCount = await commits.LongCountAsync();
        var items = commits.OrderByDescending(x => x.Commited).ThenByDescending(x=>x.Created).Skip(offset).Take(count);

        return new PagedResult<Commit>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }

    public async Task UpdateRepositoryAsync(Repository repo)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Repositories.Update(repo);
        await dbContext.SaveChangesAsync();

    }
}
