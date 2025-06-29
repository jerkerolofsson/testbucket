using TestBucket.Contracts.Insights;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Components.Reporting.Controllers;

internal class InsightsController : TenantBaseService
{
    private readonly IInsightsDataManager _manager;

    public InsightsController(IInsightsDataManager manager, AuthenticationStateProvider authenticationStateProvider) 
        :base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public string[] GetDataSourceNames() => _manager.GetDataSourceNames();

    public async Task<InsightsData<string, double>> GetDataAsync(long? projectId, InsightsDataQuery query)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetDataAsync(principal, projectId, query);
    }
}
