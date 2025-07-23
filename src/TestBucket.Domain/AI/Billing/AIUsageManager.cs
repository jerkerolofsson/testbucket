using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Billing;

/// <summary>
/// Keeps track of billing for AI usage.
/// </summary>
internal class AIUsageManager
{
    public Task AppendCostAsync(ChatUsage usage)
    {
        return Task.CompletedTask;
    }
}
