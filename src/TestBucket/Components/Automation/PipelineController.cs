

using TestBucket.Domain.Automation.Models;
using TestBucket.Domain.Automation.Services;

namespace TestBucket.Components.Automation;

internal class PipelineController : TenantBaseService
{
    private readonly IPipelineManager _pipelineManager;

    public PipelineController(AuthenticationStateProvider authenticationStateProvider, IPipelineManager pipelineManager) : base(authenticationStateProvider)
    {
        _pipelineManager = pipelineManager;
    }

    internal async Task<Pipeline?> GetPipelineByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _pipelineManager.GetPipelineByIdAsync(principal, id);
    }

    internal async Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(long testRunId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _pipelineManager.GetPipelinesForTestRunAsync(principal, testRunId);
    }
}
