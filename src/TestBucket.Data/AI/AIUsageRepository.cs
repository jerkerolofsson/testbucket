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

                OutputUSD = group.Sum(usage => usage.OutputTokenCount * usage.UsdPerMillionTokens),
                InputUSD = group.Sum(usage => usage.InputTokenCount * usage.UsdPerMillionTokens),
                TotalUSD = group.Sum(usage => usage.TotalTokenCount * usage.UsdPerMillionTokens),
            })
            .FirstOrDefaultAsync();

        return new TokenUsage
        {
            TotalTokenCount = usageData?.TotalTokenCount ?? 0,
            InputTokenCount = usageData?.InputTokenCount ?? 0,
            OutputTokenCount = usageData?.OutputTokenCount ?? 0,

            OutputSumUSD = (usageData?.OutputUSD ?? 0) / 1_000_000,
            InputSumUSD = (usageData?.InputUSD ?? 0) / 1_000_000,
            TotalSumUSD = (usageData?.TotalUSD ?? 0) / 1_000_000
        };
    }
}
