using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Migrations;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Data.Issues;
internal class IssueRepository : IIssueRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public IssueRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region Local Issues

    public async Task<PagedResult<LocalIssue>> SearchAsync(List<FilterSpecification<LocalIssue>> filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var issues = dbContext.LocalIssues.AsNoTracking().AsQueryable();

        foreach (var filter in filters)
        {
            issues = issues.Where(filter.Expression);
        }

        var totalCount = issues.LongCount();
        var page = issues.OrderByDescending(x => x.Created).Skip(offset).Take(count);

        return new PagedResult<LocalIssue>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }
    public async Task AddLocalIssueAsync(LocalIssue localIssue)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.LocalIssues.AddAsync(localIssue);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateLocalIssueAsync(LocalIssue localIssue)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.LocalIssues.Update(localIssue);
        await dbContext.SaveChangesAsync();
    }


    public async Task DeleteLocalIssueAsync(long localIssueId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.LocalIssues.Where(x => x.Id == localIssueId).ExecuteDeleteAsync();
    }

    #endregion Local Issues

    #region Links

    public async Task AddLinkedIssueAsync(LinkedIssue linkedIssue)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.LinkedIssues.AddAsync(linkedIssue);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateLinkedIssueAsync(LinkedIssue linkedIssue)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.LinkedIssues.Update(linkedIssue);
        await dbContext.SaveChangesAsync();
    }


    public async Task DeleteLinkedIssueAsync(long linkedIssueId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.LinkedIssues.Where(x=>x.Id == linkedIssueId).ExecuteDeleteAsync();
    }

    public async Task<IReadOnlyList<LinkedIssue>> GetLinkedIssuesAsync(long testCaseRun)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.LinkedIssues.AsNoTracking().Where(x => x.TestCaseRunId == testCaseRun).ToListAsync();
    }

    public async Task<PagedResult<LinkedIssue>> SearchAsync(List<FilterSpecification<LinkedIssue>> filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var issues = dbContext.LinkedIssues.AsNoTracking().AsQueryable();

        foreach(var filter in filters)
        {
            issues = issues.Where(filter.Expression);
        }

        var totalCount = issues.LongCount();
        var page = issues.OrderByDescending(x => x.Created).Skip(offset).Take(count);

        return new PagedResult<LinkedIssue>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }

    #endregion  Links

}
