using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNet.Globbing;

using Mediator;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Yaml;
using TestBucket.Domain.Code.Yaml.Models;

namespace TestBucket.Domain.Code.Services.IntegrationImpact;

public record class ResolveCommitImpactRequest(Commit Commit, ProjectArchitectureModel Model) : IRequest<CommitImpact>;

public class ResolveCommitImpactHandler : IRequestHandler<ResolveCommitImpactRequest, CommitImpact>
{
    public ValueTask<CommitImpact> Handle(ResolveCommitImpactRequest request, CancellationToken cancellationToken)
    {
        var impact = new CommitImpact();
        if(request.Commit.CommitFiles is null)
        {
            return ValueTask.FromResult(impact);
        }

        foreach(var file in request.Commit.CommitFiles)
        {
            var path = file.Path;

            foreach(var kvp in request.Model.Systems)
            {
                // Skip duplicate
                if (impact.Systems.Contains(kvp.Key)) continue;

                if (kvp.Value.Paths is not null && IsMatch(path, kvp.Value.Paths))
                {
                    impact.Systems.Add(kvp.Key);
                }
            }
            foreach (var kvp in request.Model.Layers)
            {
                // Skip duplicate
                if (impact.Layers.Contains(kvp.Key)) continue;

                if (kvp.Value.Paths is not null && IsMatch(path, kvp.Value.Paths))
                {
                    impact.Layers.Add(kvp.Key);
                }
            }
            foreach (var kvp in request.Model.Components)
            {
                // Skip duplicate
                if (impact.Components.Contains(kvp.Key)) continue;

                if (kvp.Value.Paths is not null && IsMatch(path, kvp.Value.Paths))
                {
                    impact.Components.Add(kvp.Key);
                }
            }
            foreach (var kvp in request.Model.Features)
            {
                // Skip duplicate
                if (impact.Features.Contains(kvp.Key)) continue;

                if (kvp.Value.Paths is not null && IsMatch(path, kvp.Value.Paths))
                {
                    impact.Features.Add(kvp.Key);
                }
            }

        }

        return ValueTask.FromResult(impact);
    }

    private bool IsMatch(string filePath, IEnumerable<string> globPatterns)
    {
        return GLobMatcher.IsMatch(filePath, globPatterns);
    }
}
