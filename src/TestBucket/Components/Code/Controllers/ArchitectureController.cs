
using Mediator;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Code.Services.CommitFeatureMapping;
using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Components.Code.Controllers;

/// <summary>
/// Blazor controller for architecture
/// </summary>
internal class ArchitectureController : TenantBaseService
{
    private readonly IArchitectureManager _manager;
    private readonly ICommitManager _commitManager;
    private readonly IMediator _mediator;

    public ArchitectureController(AuthenticationStateProvider authenticationStateProvider, IArchitectureManager manager, ICommitManager commitManager, IMediator mediator) : base(authenticationStateProvider)
    {
        _manager = manager;
        _commitManager = commitManager;
        _mediator = mediator;
    }

    public async Task SaveFeatureAsync(Feature feature)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateFeatureAsync(principal, feature);
    }
    
    public async Task AddCommitsToFeatureAsync(long projectId, string featureName, IEnumerable<Commit> commits)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var feature = await _manager.GetFeatureByNameAsync(principal,projectId,featureName);
        if (feature is not null)
        {
            foreach (var commit in commits)
            {
                await _mediator.Send(new MapCommitFilesToFeatureRequest(principal, commit, feature));
            }
        }
    }

    public async Task<IReadOnlyList<Feature>> GetFeaturesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetFeaturesAsync(principal, projectId);
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
