

namespace TestBucket.Domain.AI.Billing;

public interface IAIUsageManager
{
    Task AddCostAsync(ClaimsPrincipal principal, ChatUsage usage);
    Task<TokenUsage> GetTotalUsageAsync(ClaimsPrincipal principal, DateTimeOffset from, DateTimeOffset until);
}