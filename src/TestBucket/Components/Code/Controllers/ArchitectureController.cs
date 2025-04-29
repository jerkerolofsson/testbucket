
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
    private readonly ICommitManager _commitManager;

    public ArchitectureController(AuthenticationStateProvider authenticationStateProvider, IArchitectureManager manager, ICommitManager commitManager) : base(authenticationStateProvider)
    {
        _manager = manager;
        _commitManager = commitManager;
    }

    public async Task AddCommitsToFeatureAsync(long projectId, string featureName, IEnumerable<Commit> commits)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var feature = await _manager.GetFeatureByNameAsync(principal,projectId,featureName);
        if (feature is not null)
        {
            await _manager.AddCommitsToFeatureAsync(principal, feature, commits);

            // Update the commits with the added feature
            foreach (var commit in commits)
            {
                commit.FeatureNames ??= [];
                if(!commit.FeatureNames.Contains(featureName))
                {
                    commit.FeatureNames.Add(featureName);
                    await _commitManager.UpdateCommitAsync(principal, commit);
                }
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
