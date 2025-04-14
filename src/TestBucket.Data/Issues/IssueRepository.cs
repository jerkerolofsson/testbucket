using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Data.Migrations;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Data.Issues;
internal class IssueRepository : IIssueRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public IssueRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

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
}
