using System.Linq;
using TestBucket.Contracts;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.UnitTests.Testing.Fakes;
internal class FakeTestRunRepository : ITestRunRepository
{
    /// <summary>
    /// In memory 
    /// </summary>
    private readonly List<TestRun> _testRuns = new();

    public Task AddTestRunAsync(TestRun testRun)
    {
        _testRuns.Add(testRun);
        return Task.CompletedTask;
    }

    public Task DeleteTestRunAsync(TestRun testRun)
    {
        _testRuns.Remove(testRun);
        return Task.CompletedTask;
    }

    public Task<TestRun?> GetTestRunByIdAsync(string tenantId, long id)
    {
        var testRun = _testRuns.FirstOrDefault(tr => tr.TenantId == tenantId && tr.Id == id);
        return Task.FromResult(testRun);
    }

    public Task<PagedResult<TestRun>> SearchTestRunsAsync(IReadOnlyList<FilterSpecification<TestRun>> filters, int offset, int count)
    {
        var filteredRuns = _testRuns.AsQueryable();

        foreach (var filter in filters)
        {
            filteredRuns = filteredRuns.Where(filter.Expression);
        }

        var totalCount = filteredRuns.Count();
        var pagedRuns = filteredRuns.OrderByDescending(x=>x.Modified).Skip(offset).Take(count).ToArray();
        var result = new PagedResult<TestRun>
        {
            TotalCount = totalCount,
            Items = pagedRuns
        };

        return Task.FromResult(result);
    }

    public Task UpdateTestRunAsync(TestRun testRun)
    {
        _testRuns.RemoveAll(tr => tr.Id == testRun.Id);
        _testRuns.Add(testRun);
        return Task.CompletedTask;
    }
}
