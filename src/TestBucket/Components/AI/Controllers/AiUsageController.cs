
using TestBucket.Domain.AI.Billing;

namespace TestBucket.Components.AI.Controllers;

internal class AiUsageController : TenantBaseService
{
    private readonly IAIUsageManager _manager;

    public AiUsageController(AuthenticationStateProvider authenticationStateProvider, IAIUsageManager manager) : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task<TokenUsage> GetYearlyUsageAsync(int year)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        DateTimeOffset from = new DateTimeOffset(year,1,1,0,0,0,0,TimeSpan.Zero);
        DateTimeOffset until = new DateTimeOffset(year, 12, 31, 23, 59, 59, 999, TimeSpan.Zero);

        return await _manager.GetTotalUsageAsync(principal, from,until);
    }
}
