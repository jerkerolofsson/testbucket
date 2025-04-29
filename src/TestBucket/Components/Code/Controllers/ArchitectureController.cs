
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Components.Code.Controllers;

/// <summary>
/// Blazor controller for architecture
/// </summary>
internal class ArchitectureController : TenantBaseService
{
    private readonly IArchitectureManager _manager;

    public ArchitectureController(AuthenticationStateProvider authenticationStateProvider, IArchitectureManager manager) : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task<Component?> GetComponentByNameAsync(TestProject project, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetComponentByNameAsync(principal, project.Id, name);
    }
    public async Task<ProjectArchitectureModel> GetProductArchitectureAsync(TestProject project)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetProductArchitectureAsync(principal, project);
    }
    public async Task ImportProductArchitectureAsync(TestProject project, ProjectArchitectureModel model)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.ImportProductArchitectureAsync(principal, project, model);
    }
}
