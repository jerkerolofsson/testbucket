using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics;
public class HeuristicsManager : IHeuristicsManager
{
    private readonly IHeuristicsRepository _repository;
    private readonly TimeProvider _timeProvider;

    public HeuristicsManager(TimeProvider timeProvider, IHeuristicsRepository repository)
    {
        _timeProvider = timeProvider;
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task AddAsync(ClaimsPrincipal principal, Heuristic heuristic)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Heuristic, PermissionLevel.Write);

        heuristic.TenantId = principal.GetTenantIdOrThrow();
        heuristic.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        heuristic.ModifiedBy = heuristic.CreatedBy;
        heuristic.Created = _timeProvider.GetUtcNow();
        heuristic.Modified = _timeProvider.GetUtcNow();
        await _repository.AddAsync(heuristic);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(ClaimsPrincipal principal, Heuristic heuristic)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Heuristic, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(heuristic.TenantId);
        await _repository.DeleteAsync(heuristic.Id);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<Heuristic>> SearchAsync(ClaimsPrincipal principal, FilterSpecification<Heuristic>[] filters, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Heuristic, PermissionLevel.Read);

        // Always filter by tenant
        var tenantId = principal.GetTenantIdOrThrow();
        var allFilters = filters?.ToList() ?? new List<FilterSpecification<Heuristic>>();
        allFilters.Add(new FilterByTenant<Heuristic>(tenantId));
        return await _repository.SearchAsync(allFilters.ToArray(), offset, count);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(ClaimsPrincipal principal, Heuristic heuristic)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Heuristic, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(heuristic.TenantId);

        heuristic.ModifiedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        heuristic.Modified = _timeProvider.GetUtcNow();
        await _repository.UpdateAsync(heuristic);
    }
}