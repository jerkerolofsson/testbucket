using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Billing;

/// <summary>
/// Keeps track of billing for AI usage.
/// </summary>
internal class AIUsageManager : IAIUsageManager
{
    private readonly TimeProvider _timeProvider;
    private readonly IAIUsageRepository _repository;

    public AIUsageManager(IAIUsageRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<TokenUsage> GetTotalUsageAsync(ClaimsPrincipal principal, DateTimeOffset from, DateTimeOffset until)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetTotalUsageAsync(tenantId, from, until);
    }

    public async Task AddCostAsync(ClaimsPrincipal principal, ChatUsage usage)
    {
        usage.TenantId = principal.GetTenantIdOrThrow();
        usage.CreatedBy = usage.ModifiedBy = principal.Identity?.Name ?? "unknown";
        usage.Created = usage.Modified = _timeProvider.GetUtcNow();

        await _repository.AddAsync(usage);
    }
}
