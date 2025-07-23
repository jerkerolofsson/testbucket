using System;
namespace TestBucket.Domain.AI.Billing;
public interface IAIUsageRepository
{
    /// <summary>
    /// Adds a new usage record to the repository.
    /// </summary>
    /// <param name="usage"></param>
    /// <returns></returns>
    Task AddAsync(ChatUsage usage);

    /// <summary>
    /// Calculates the total (sum) usage within the specified inclusive dates
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="from"></param>
    /// <param name="until"></param>
    /// <returns></returns>
    Task<TokenUsage> GetTotalUsageAsync(string tenantId, DateTimeOffset from, DateTimeOffset until);
}
