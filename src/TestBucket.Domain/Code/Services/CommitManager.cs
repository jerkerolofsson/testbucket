using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services.IntegrationImpact;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Services;
public class CommitManager : ICommitManager
{
    private readonly ICommitRepository _repo;
    private readonly IMediator _mediator;
    private readonly IArchitectureManager _architectureManager;
    private readonly IArchitectureRepository _architectureRepository;
    private readonly IProjectManager _projectManager;

    public CommitManager(ICommitRepository repo, IMediator mediator, IArchitectureManager architectureManager, IProjectManager projectManager, IArchitectureRepository architectureRepository)
    {
        _repo = repo;
        _mediator = mediator;
        _architectureManager = architectureManager;
        _projectManager = projectManager;
        _architectureRepository = architectureRepository;
    }

    public Task<PagedResult<Commit>> BrowseCommitsAsync(ClaimsPrincipal principal, long projectId, int offset, int count)
    {
        FilterSpecification<Commit>[] filters = [
            new FilterByTenant<Commit>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Commit>(projectId)
            ];

        return _repo.SearchCommitsAsync(filters, offset, count);
    }

    public async Task AddCommitAsync(ClaimsPrincipal principal, Commit commit)
    {
        ArgumentNullException.ThrowIfNull(commit.TeamId);
        ArgumentNullException.ThrowIfNull(commit.TestProjectId);
        var projectId = commit.TestProjectId.Value;

        commit.TenantId = principal.GetTenantIdOrThrow();
        commit.Created = DateTimeOffset.UtcNow;
        commit.Modified = DateTimeOffset.UtcNow;
        commit.CreatedBy = principal.Identity?.Name;
        commit.ModifiedBy = principal.Identity?.Name;

        await _repo.DeleteCommitByShaAsync(commit.Sha);
        await _repo.AddCommitAsync(commit);

        if (commit.CommitFiles is not null && commit.CommitFiles.Count > 0)
        {
            var project = commit.TestProject ?? await _projectManager.GetTestProjectByIdAsync(principal, commit.TestProjectId.Value);
            if (project is not null)
            {
                var model = await _architectureManager.GetProductArchitectureAsync(principal, project);

                // Scan for impacted feature/component/layer
                var impact = await _mediator.Send(new ResolveCommitImpactRequest(commit, model));
                foreach (var name in impact.Systems)
                {
                    var component = await _architectureManager.GetSystemByNameAsync(principal, projectId, name);
                    if (component is not null)
                    {
                        commit.SystemNames ??= new();
                        commit.SystemNames.Add(component.Name);
                    }
                }
                foreach (var name in impact.Components)
                {
                    var component = await _architectureManager.GetComponentByNameAsync(principal, projectId, name);
                    if (component is not null)
                    {
                        commit.ComponentNames ??= new();
                        commit.ComponentNames.Add(component.Name);
                    }
                }
                foreach (var name in impact.Features)
                {
                    var feature = await _architectureManager.GetFeatureByNameAsync(principal, projectId, name);
                    if (feature is not null)
                    {
                        commit.FeatureNames ??= new();
                        commit.FeatureNames.Add(feature.Name);
                    }
                }
                foreach (var name in impact.Layers)
                {
                    var layer = await _architectureManager.GetLayerByNameAsync(principal, projectId, name);
                    if (layer is not null)
                    {
                        commit.LayerNames ??= new();
                        commit.LayerNames.Add(layer.Name);
                    }
                }

                await _repo.UpdateCommitAsync(commit);
            }
        }
    }

    public async Task AddRepositoryAsync(ClaimsPrincipal principal, Repository repo)
    {
        ArgumentNullException.ThrowIfNull(repo.TeamId);
        ArgumentNullException.ThrowIfNull(repo.TestProjectId);
        repo.TenantId = principal.GetTenantIdOrThrow();
        repo.Created = DateTimeOffset.UtcNow;
        repo.Modified = DateTimeOffset.UtcNow;
        repo.CreatedBy = principal.Identity?.Name;
        repo.ModifiedBy = principal.Identity?.Name;
        await _repo.AddRepositoryAsync(repo);
    }

    public async Task<Repository?> GetRepoByExternalSystemAsync(ClaimsPrincipal principal, long id)
    {
        return await _repo.GetRepoByExternalSystemAsync(id);
    }

    public async Task UpdateRepositoryAsync(ClaimsPrincipal principal, Repository repo)
    {
        repo.Modified = DateTimeOffset.UtcNow;
        repo.ModifiedBy = principal.Identity?.Name;
        await _repo.UpdateRepositoryAsync(repo);
    }
}
