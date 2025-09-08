using TestBucket.Contracts;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.UnitTests.Issues.Fakes;

internal class FakeIssueRepository : IIssueRepository
{
    private readonly List<LocalIssue> _localIssues = new();
    private readonly List<LinkedIssue> _linkedIssues = new();
    private long _idCounter = 0;

    public Task AddLinkedIssueAsync(LinkedIssue linkedIssue)
    {
        linkedIssue.Id = ++_idCounter;
        _linkedIssues.Add(linkedIssue);
        return Task.CompletedTask;
    }

    public Task AddLocalIssueAsync(LocalIssue localIssue)
    {
        localIssue.Id = ++_idCounter;
        _localIssues.Add(localIssue);
        return Task.CompletedTask;
    }

    public Task DeleteLinkedIssueAsync(long linkedIssueId)
    {
        _linkedIssues.RemoveAll(issue => issue.Id == linkedIssueId);
        return Task.CompletedTask;
    }

    public Task DeleteLocalIssueAsync(long localIssueId)
    {
        _localIssues.RemoveAll(issue => issue.Id == localIssueId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<LinkedIssue>> GetLinkedIssuesAsync(long testCaseRun)
    {
        var result = _linkedIssues.Where(issue => issue.TestCaseRunId == testCaseRun).ToList();
        return Task.FromResult((IReadOnlyList<LinkedIssue>)result);
    }

    public Task<PagedResult<LinkedIssue>> SearchAsync(List<FilterSpecification<LinkedIssue>> filters, int count, int offset)
    {
        var filtered = _linkedIssues.AsQueryable();
        foreach (var filter in filters)
        {
            filtered = filtered.Where(filter.Expression);
        }

        var result = filtered.OrderBy(x=>x.Created).Skip(offset).Take(count).ToList();
        return Task.FromResult(new PagedResult<LinkedIssue>
        {
            Items = result.ToArray(),
            TotalCount = filtered.Count()
        });
    }

    public Task<PagedResult<LocalIssue>> SearchAsync(List<FilterSpecification<LocalIssue>> filters, int offset, int count)
    {
        var filtered = _localIssues.AsQueryable();
        foreach (var filter in filters)
        {
            filtered = filtered.Where(filter.Expression);
        }

        var result = filtered.OrderBy(x => x.Created).Skip(offset).Take(count).ToArray();
        return Task.FromResult(new PagedResult<LocalIssue>
        {
            Items = result,
            TotalCount = filtered.LongCount()
        });
    }

    public Task UpdateLinkedIssueAsync(LinkedIssue linkedIssue)
    {
        var index = _linkedIssues.FindIndex(issue => issue.Id == linkedIssue.Id);
        if (index >= 0)
        {
            _linkedIssues[index] = linkedIssue;
        }
        return Task.CompletedTask;
    }

    public Task UpdateLocalIssueAsync(LocalIssue localIssue)
    {
        var index = _localIssues.FindIndex(issue => issue.Id == localIssue.Id);
        if (index >= 0)
        {
            _localIssues[index] = localIssue;
        }
        return Task.CompletedTask;
    }

    public Task<InsightsData<MappedIssueState, int>> GetIssueCountPerStateAsync(IEnumerable<FilterSpecification<LocalIssue>> filters)
    {
        var filtered = _localIssues.AsQueryable();
        foreach (var filter in filters)
        {
            filtered = filtered.Where(filter.Expression);
        }

        var insightsData = new InsightsData<MappedIssueState, int>();
        foreach (var group in filtered.GroupBy(issue => issue.MappedState))
        {
            var series = new InsightsSeries<MappedIssueState, int>
            {
                Name = group.Key?.ToString() ?? "n/a"
            };

            foreach (var issue in group)
            {
                var key = group.Key ?? MappedIssueState.Open;
                series.Add(key, group.Count());
            }

            insightsData.Add(series);
        }
        return Task.FromResult(insightsData);
    }

    public Task<InsightsData<string, int>> GetIssueCountPerFieldAsync(IEnumerable<FilterSpecification<LocalIssue>> filters, long fieldDefinitionId)
    {
        // Mock implementation for testing
        return Task.FromResult(new InsightsData<string, int>());
    }

    public Task<InsightsData<DateOnly, int>> GetClosedIssuesPerDay(IEnumerable<FilterSpecification<LocalIssue>> filters)
    {
        // Mock implementation for testing
        return Task.FromResult(new InsightsData<DateOnly, int>());
    }

    public Task<InsightsData<DateOnly, int>> GetCreatedIssuesPerDay(IEnumerable<FilterSpecification<LocalIssue>> filters)
    {
        // Mock implementation for testing
        return Task.FromResult(new InsightsData<DateOnly, int>());
    }

    public Task<PagedResult<LocalIssue>> SemanticSearchAsync(ReadOnlyMemory<float> embeddingVector, List<FilterSpecification<LocalIssue>> filters, int offset, int count)
    {
        // Mock implementation for testing
        return Task.FromResult(new PagedResult<LocalIssue>
        {
            Items = Array.Empty<LocalIssue>(),
            TotalCount = 0
        });
    }
}