using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.UnitTests.TestResources.Fakes;
using System.Collections.Concurrent;

using TestBucket.Contracts;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Models;

internal class FakeTestResourceRepository : ITestResourceRepository
{
    private readonly ConcurrentDictionary<long, TestResource> _resources = new();
    private long _currentId = 0;

    public Task AddAsync(TestResource resource)
    {
        resource.Id = Interlocked.Increment(ref _currentId);
        _resources[resource.Id] = resource;
        return Task.CompletedTask;
    }

    public Task<TestResource?> GetByIdAsync(string tenantId, long id)
    {
        _resources.TryGetValue(id, out var resource);
        return Task.FromResult(resource);
    }

    public Task DeleteAsync(long id)
    {
        _resources.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<PagedResult<TestResource>> SearchAsync(FilterSpecification<TestResource>[] filters, int offset, int count)
    {
        var filteredResources = _resources.Values.AsQueryable();

        foreach (var filter in filters)
        {
            filteredResources = filteredResources.Where(filter.Expression);
        }

        var pagedResources = filteredResources
            .Skip(offset)
            .Take(count)
            .ToList();

        return Task.FromResult(new PagedResult<TestResource>
        {
            Items = pagedResources.ToArray(),
            TotalCount = _resources.Count
        });
    }

    public Task UpdateAsync(TestResource resource)
    {
        if (_resources.ContainsKey(resource.Id))
        {
            _resources[resource.Id] = resource;
        }
        return Task.CompletedTask;
    }
}
