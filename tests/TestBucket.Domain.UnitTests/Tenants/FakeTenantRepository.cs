using System.Collections.Concurrent;

using OneOf;

using TestBucket.Contracts;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.UnitTests.Tenants;

internal class FakeTenantRepository : ITenantRepository
{
    private readonly ConcurrentDictionary<string, Tenant> _tenants = new();

    public Task<OneOf<Tenant, AlreadyExistsError>> CreateAsync(string name, string tenantId)
    {
        if (_tenants.ContainsKey(tenantId))
        {
            return Task.FromResult<OneOf<Tenant, AlreadyExistsError>>(new AlreadyExistsError());
        }

        var tenant = new Tenant
        {
            Id = tenantId,
            Name = name
        };

        _tenants[tenantId] = tenant;
        return Task.FromResult<OneOf<Tenant, AlreadyExistsError>>(tenant);
    }

    public Task<bool> ExistsAsync(string tenantId)
    {
        return Task.FromResult(_tenants.ContainsKey(tenantId));
    }

    public Task<Tenant?> GetTenantByIdAsync(string tenantId)
    {
        _tenants.TryGetValue(tenantId, out var tenant);
        return Task.FromResult(tenant);
    }

    public Task<PagedResult<Tenant>> SearchAsync(SearchQuery query)
    {
        var items = _tenants.Values
            .Where(t => string.IsNullOrEmpty(query.Text) || t.Name.Contains(query.Text, StringComparison.OrdinalIgnoreCase))
            .Skip(query.Offset)
            .Take(query.Count)
            .ToArray();

        return Task.FromResult(new PagedResult<Tenant>
        {
            TotalCount = _tenants.Count,
            Items = items
        });
    }

    public Task UpdateTenantAsync(Tenant tenant)
    {
        if (_tenants.ContainsKey(tenant.Id))
        {
            _tenants[tenant.Id] = tenant;
        }
        return Task.CompletedTask;
    }

    public Task DeleteTenantAsync(string tenantId)
    {
        _tenants.TryRemove(tenantId, out _);
        return Task.CompletedTask;
    }
}