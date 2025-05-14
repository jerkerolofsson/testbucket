using TestBucket.Data.Sequence;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Issues;
internal class IssueRepository : IIssueRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ISequenceGenerator _sequenceGenerator;
    public IssueRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory, ISequenceGenerator sequenceGenerator)
    {
        _dbContextFactory = dbContextFactory;
        _sequenceGenerator = sequenceGenerator;
    }

    #region Local Issues

    public async Task<PagedResult<LocalIssue>> SearchAsync(List<FilterSpecification<LocalIssue>> filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var issues = dbContext.LocalIssues.AsNoTracking()
            .Include(x => x.IssueFields)
            .Include(x=>x.Comments).AsQueryable();

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

    private async ValueTask<int> GetMaxSequenceNumber(string tenantId, long projectId, CancellationToken cancellationToken)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var lastestSequenceNumber = await dbContext.LocalIssues.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.SequenceNumber != null)
            .OrderByDescending(x => x.SequenceNumber).Select(x=>x.SequenceNumber).Take(1).FirstOrDefaultAsync();

        return lastestSequenceNumber ?? 0;
    }

    public async Task AddLocalIssueAsync(LocalIssue localIssue)
    {
        ArgumentNullException.ThrowIfNull(localIssue.TenantId);
        ArgumentNullException.ThrowIfNull(localIssue.TestProjectId);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Sequence number only used for internal issues
        if (localIssue.ExternalId is null)
        {
            var project = await dbContext.Projects.AsNoTracking().Where(x => x.Id == localIssue.TestProjectId).FirstAsync();
            localIssue.SequenceNumber = await _sequenceGenerator.GenerateSequenceNumberAsync(localIssue.TenantId, localIssue.TestProjectId.Value, "issue", GetMaxSequenceNumber, default);
            localIssue.ExternalDisplayId = project.ShortName + '-' + localIssue.SequenceNumber;
        }

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
