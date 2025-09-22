using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.UnitTests.Automation.Fakes;

internal class FakeRunnerRepository : IRunnerRepository
{
    private readonly List<Runner> _runners = new();

    public Task AddAsync(Runner runner)
    {
        _runners.Add(runner);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Runner runner)
    {
        _runners.RemoveAll(x => x.Id == runner.Id);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Runner>> GetAllAsync(string tenantId)
    {
        return Task.FromResult<IReadOnlyList<Runner>>(_runners.Where(x => x.TenantId == tenantId).ToList());
    }

    public Task<Runner?> GetByIdAsync(string tenantId, string id)
    {
        return Task.FromResult(_runners.FirstOrDefault(x => x.TenantId == tenantId && x.Id == id));
    }

    public Task UpdateAsync(Runner runner)
    {
        _runners.RemoveAll(x => x.Id == runner.Id);
        _runners.Add(runner);
        return Task.CompletedTask;
    }
}
