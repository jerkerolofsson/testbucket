using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI.Billing;

namespace TestBucket.Data.AI;
internal class AIUsageRepository : IAIUsageRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public AIUsageRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task AddAsync(ChatUsage usage)
    {
        using var context = _factory.CreateDbContext();
        context.ChatUsages.Add(usage);
        await context.SaveChangesAsync();
    }

    public async Task<TokenUsage> GetTotalUsageAsync(string tenantId, DateTimeOffset from, DateTimeOffset until)
    {
        using var context = _factory.CreateDbContext();

        var usageData = await context.ChatUsages
            .Where(usage => usage.TenantId == tenantId && usage.Created >= from && usage.Created <= until)
            .GroupBy(_ => 1) // Grouping by a constant to aggregate all matching rows
            .Select(group => new
            {
                TotalTokenCount = group.Sum(usage => usage.TotalTokenCount),
                InputTokenCount = group.Sum(usage => usage.InputTokenCount),
                OutputTokenCount = group.Sum(usage => usage.OutputTokenCount),

                InputUSD = group.Sum(usage => usage.InputTokenCount * usage.UsdPerMillionInputTokens / 1_000_000),
                OutputUSD = group.Sum(usage => usage.OutputTokenCount * usage.UsdPerMillionOutputTokens / 1_000_000),
            })
            .FirstOrDefaultAsync();

        var inputTokens = usageData?.InputTokenCount ?? 0;
        var outputTokens = usageData?.OutputTokenCount ?? 0;
        var totalTokens = usageData?.TotalTokenCount ?? 0;

        return new TokenUsage
        {
            TotalTokenCount = totalTokens,
            InputTokenCount = inputTokens,
            OutputTokenCount = outputTokens,

            OutputSumUSD = usageData?.OutputUSD ?? 0,
            InputSumUSD = usageData?.InputUSD ?? 0,
            TotalSumUSD = (usageData?.OutputUSD ?? 0) + (usageData?.InputUSD ?? 0)
        };
    }
}
