using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Automation;

internal class RunnersController : TenantBaseService
{
    private readonly IRunnerManager _manager;

    public RunnersController(IRunnerManager manager, AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task<IReadOnlyList<Runner>> GetRunnersAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetAllAsync(principal);
    }

    public async Task RemoveRunnerAsync(Runner runner)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        
        await _manager.RemoveAsync(principal, runner);
    }
}
