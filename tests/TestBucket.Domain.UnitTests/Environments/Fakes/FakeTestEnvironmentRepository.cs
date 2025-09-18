using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;

namespace TestBucket.Domain.UnitTests.Environments.Fakes;

internal class FakeTestEnvironmentRepository : ITestEnvironmentRepository
{
    private readonly List<TestEnvironment> _environments = new();
    private long _counter = 0;

    public bool ThrowOnAdd { get; internal set; }

    public Task AddTestEnvironmentAsync(TestEnvironment testEnvironment)
    {
        if(ThrowOnAdd)
        {
            throw new InvalidOperationException("ThrowOnAdd is true");
        }

        testEnvironment.Id = ++_counter;

        _environments.Add(testEnvironment);
        return Task.CompletedTask;
    }

    public Task DeleteTestEnvironmentAsync(long id)
    {
        var env = _environments.FirstOrDefault(e => e.Id == id);
        if (env != null)
            _environments.Remove(env);
        return Task.CompletedTask;
    }

    public Task UpdateTestEnvironmentAsync(TestEnvironment testEnvironment)
    {
        var index = _environments.FindIndex(e => e.Id == testEnvironment.Id);
        if (index >= 0)
            _environments[index] = testEnvironment;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(string tenantId)
    {
        var result = _environments.Where(e => e.TenantId == tenantId).ToList();
        return Task.FromResult((IReadOnlyList<TestEnvironment>)result);
    }

    public Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(IEnumerable<FilterSpecification<TestEnvironment>> filters)
    {
        IEnumerable<TestEnvironment> result = _environments;
        foreach (var filter in filters)
        {
            result = result.Where(e => filter.IsMatch(e));
        }
        return Task.FromResult((IReadOnlyList<TestEnvironment>)result.ToList());
    }

    public Task<TestEnvironment?> GetTestEnvironmentByIdAsync(string tenantId, long id)
    {
        var env = _environments.FirstOrDefault(e =>
            e.Id == id &&
            e.TenantId == tenantId);
        return Task.FromResult(env);
    }
}