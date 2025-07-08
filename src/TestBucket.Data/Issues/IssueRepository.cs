using System.Linq;

using Pgvector.EntityFrameworkCore;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Data.Sequence;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;

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


    /// <summary>
    /// Returns number of issues per state
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public async Task<InsightsData<MappedIssueState, int>> GetIssueCountPerStateAsync(IEnumerable<FilterSpecification<LocalIssue>> filters)
    {
        var data = new InsightsData<MappedIssueState, int>() { Name = "issue-count" };
        var series = new InsightsSeries<MappedIssueState, int>() { Name = "count-by-state" };
        data.Add(series);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var localIsses = dbContext.LocalIssues.AsQueryable();

        foreach (var filter in filters)
        {
            localIsses = localIsses.Where(filter.Expression);
        }

        var issues = await localIsses.GroupBy(x => x.MappedState).
            Select(g => new { MappedState = g.Key, Count = g.Count() }).ToListAsync();

        foreach (var item in issues)
        {
            if (item.MappedState is not null)
            {
                var name = item.MappedState.Value;
                series.Add(name, item.Count);
            }
        }

        return data;
    }

    /// <summary>
    /// Returns number of created issues per date
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public async Task<InsightsData<DateOnly, int>> GetCreatedIssuesPerDay(IEnumerable<FilterSpecification<LocalIssue>> filters)
    {
        var data = new InsightsData<DateOnly, int>() { Name = "created-issues" };
        var series = new InsightsSeries<DateOnly, int>() { Name = "created-issues-by-day" };
        data.Add(series);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var localIsses = dbContext.LocalIssues.AsQueryable();

        foreach (var filter in filters)
        {
            localIsses = localIsses.Where(filter.Expression);
        }

        var issues = await localIsses.GroupBy(x => new { x.Created.Year, x.Created.Month, x.Created.Day }).
            Select(g => new { Date = g.Key, Count = g.Count() }).ToListAsync();

        foreach (var item in issues)
        {
            var date = new DateOnly(item.Date.Year, item.Date.Month, item.Date.Day);
            series.Add(date, item.Count);
        }

        return data;
    }


    /// <summary>
    /// Returns number of created issues per date
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public async Task<InsightsData<DateOnly, int>> GetClosedIssuesPerDay(IEnumerable<FilterSpecification<LocalIssue>> filters)
    {
        var data = new InsightsData<DateOnly, int>() { Name = "closed-issues" };
        var series = new InsightsSeries<DateOnly, int>() { Name = "closed-issues-by-day" };
        data.Add(series);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var localIssues = dbContext.LocalIssues.AsQueryable();
        localIssues = localIssues.Where(x => x.Closed != null);

        foreach (var filter in filters)
        {
            localIssues = localIssues.Where(filter.Expression);
        }

        var issues = await localIssues.GroupBy(x => new { x.Closed!.Value.Year, x.Closed!.Value.Month, x.Closed!.Value.Day }).
            Select(g => new { Date = g.Key, Count = g.Count() }).ToListAsync();
            
        foreach (var item in issues)
        {
            var date = new DateOnly(item.Date.Year, item.Date.Month, item.Date.Day);
            series.Add(date, item.Count);
        }

        return data;
    }

    /// <summary>
    /// Returns number of issues per field
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="fieldDefinitionId"></param>
    /// <returns></returns>
    public async Task<InsightsData<string,int>> GetIssueCountPerFieldAsync(IEnumerable<FilterSpecification<LocalIssue>> filters, long fieldDefinitionId)
    {
        var data = new InsightsData<string, int>() { Name = "issue-count" };
        var series = new InsightsSeries<string, int>() { Name = "count-by-field" };
        data.Add(series);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var localIsses = dbContext.LocalIssues
            .Include(x => x.IssueFields)
            .Where(x => x.IssueFields!.Any(f => f.FieldDefinitionId == fieldDefinitionId))
            .AsQueryable();

        foreach (var filter in filters)
        {
            localIsses = localIsses.Where(filter.Expression);
        }

        var issues = await localIsses.GroupBy(x => new { x.IssueFields!.First(x => x.FieldDefinitionId == fieldDefinitionId).StringValue }).
            Select(g => new { FieldValue = g.Key.StringValue, Count = g.Count() }).ToListAsync();

        foreach (var item in issues)
        {
            var name = item.FieldValue ?? "(null)";
            series.Add(name, item.Count);
        }

        return data;
    }

    public async Task<PagedResult<LocalIssue>> SemanticSearchAsync(ReadOnlyMemory<float> embeddingVector, List<FilterSpecification<LocalIssue>> filters, int offset, int count)
    {
        var vector = new Pgvector.Vector(embeddingVector);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var issues = dbContext.LocalIssues.AsNoTracking()
            .Include(x => x.Comments)
            .Include(x => x.IssueFields!).ThenInclude(y => y.FieldDefinition)
            .AsQueryable();

        foreach (var filter in filters)
        {
            issues = issues.Where(filter.Expression);
        }
        issues = issues.Where(x => x.Embedding != null);

        var totalCount = issues.LongCount();

        var page = issues.Select(x=> new { Issue = x, Distance = x.Embedding!.CosineDistance(vector) })
            .OrderBy(x => x.Distance)
            .Skip(offset)
            .Take(count)
            .Select(x => x.Issue);

        return new PagedResult<LocalIssue>()
        {
            TotalCount = totalCount,
            Items = await page.ToArrayAsync()
        };
    }
    public async Task<PagedResult<LocalIssue>> SearchAsync(List<FilterSpecification<LocalIssue>> filters, int offset, int count)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var issues = dbContext.LocalIssues.AsNoTracking()
            .Include(x=>x.Comments)
            .Include(x => x.IssueFields!).ThenInclude(y=>y.FieldDefinition)
            .AsQueryable();

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
        localIssue.Created = localIssue.Created.ToUniversalTime();
        localIssue.Modified = localIssue.Modified.ToUniversalTime();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.LocalIssues.Update(localIssue);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteLocalIssueAsync(long localIssueId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach (var issue in dbContext.LinkedIssues.Where(x => x.LocalIssueId == localIssueId))
        {
            dbContext.Remove(issue);
        }
        foreach (var issue in dbContext.LocalIssues
            .Include(x=>x.Comments)
            .Include(x=>x.IssueFields)
            .Where(x => x.Id == localIssueId))
        {
            dbContext.Remove(issue);
        }
        await dbContext.SaveChangesAsync();
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
