
using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Domain.Metrics;
internal class MetricsManager : IMetricsManager
{
    private readonly TimeProvider _timeProvider;
    private readonly IMetricsRepository _repository;

    public MetricsManager(IMetricsRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task AddMetric(ClaimsPrincipal principal, Metric metric)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Write);

        metric.TenantId = principal.GetTenantIdOrThrow();
        metric.Modified = _timeProvider.GetUtcNow();
        metric.CreatedBy = principal.Identity?.Name;
        metric.ModifiedBy = principal.Identity?.Name;

        await _repository.AddMetric(metric);
    }
}
