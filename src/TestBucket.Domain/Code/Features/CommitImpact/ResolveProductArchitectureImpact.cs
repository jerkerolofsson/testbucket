using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Code.Events;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Code.Services.IntegrationImpact;
using TestBucket.Domain.Projects;

namespace TestBucket.Domain.Code.Features.CommitImpact;
internal class ResolveProductArchitectureImpact : INotificationHandler<CommitAddedEvent>
{
    private readonly ICommitManager _commitManager;
    private readonly IArchitectureManager _architectureManager;
    private readonly IProjectManager _projectManager;
    private readonly IMediator _mediator;

    public ResolveProductArchitectureImpact(ICommitManager commitManager, IArchitectureManager architectureManager, IProjectManager projectManager, IMediator mediator)
    {
        _commitManager = commitManager;
        _architectureManager = architectureManager;
        _projectManager = projectManager;
        _mediator = mediator;
    }

    public async ValueTask Handle(CommitAddedEvent notification, CancellationToken cancellationToken)
    {
        var commit = notification.Commit;
        var principal = notification.Principal;

        if (commit.CommitFiles is not null && commit.CommitFiles.Count > 0 && commit.TestProjectId is not null)
        {
            var project = commit.TestProject ?? await _projectManager.GetTestProjectByIdAsync(principal, commit.TestProjectId.Value);
            if (project is not null)
            {
                var model = await _architectureManager.GetProductArchitectureAsync(principal, project);

                // Scan for impacted feature/component/layer
                var impact = await _mediator.Send(new ResolveCommitImpactRequest(commit, model));
                foreach (var name in impact.Systems)
                {
                    var component = await _architectureManager.GetSystemByNameAsync(principal, project.Id, name);
                    if (component is not null)
                    {
                        commit.SystemNames ??= new();
                        commit.SystemNames.Add(component.Name);
                    }
                }
                foreach (var name in impact.Components)
                {
                    var component = await _architectureManager.GetComponentByNameAsync(principal, project.Id, name);
                    if (component is not null)
                    {
                        commit.ComponentNames ??= new();
                        commit.ComponentNames.Add(component.Name);
                    }
                }
                foreach (var name in impact.Features)
                {
                    var feature = await _architectureManager.GetFeatureByNameAsync(principal, project.Id, name);
                    if (feature is not null)
                    {
                        commit.FeatureNames ??= new();
                        commit.FeatureNames.Add(feature.Name);
                    }
                }
                foreach (var name in impact.Layers)
                {
                    var layer = await _architectureManager.GetLayerByNameAsync(principal, project.Id, name);
                    if (layer is not null)
                    {
                        commit.LayerNames ??= new();
                        commit.LayerNames.Add(layer.Name);
                    }
                }

                await _commitManager.UpdateCommitAsync(principal, commit);
            }
        }
    }
}
