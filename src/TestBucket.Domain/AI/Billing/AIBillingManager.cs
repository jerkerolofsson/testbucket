using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Billing;

/// <summary>
/// Keeps track of billing for AI usage.
/// </summary>
internal class AIBillingManager
{
    public Task RecordCostAsync(ClaimsPrincipal principal, string provider, string model, int tokenCount)
    {
        return Task.CompletedTask;
    }
}
