using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.Code.Services.CommitFeatureMapping;
public record class MapCommitFilesToFeatureRequest(ClaimsPrincipal Principal, Commit Commit, Feature Feature) : IRequest;

public class MapCommitFilesToFeatureHandler : IRequestHandler<MapCommitFilesToFeatureRequest>
{
    private readonly ICommitManager _commitManager;
    private readonly IArchitectureManager _architectureManager;

    public MapCommitFilesToFeatureHandler(ICommitManager commitManager, IArchitectureManager architectureManager)
    {
        _commitManager = commitManager;
        _architectureManager = architectureManager;
    }

    public async ValueTask<Unit> Handle(MapCommitFilesToFeatureRequest request, CancellationToken cancellationToken)
    {
        var commit = request.Commit;
        var feature = request.Feature;

        feature.GlobPatterns ??= [];
        bool changed = false;
        if (commit.CommitFiles is null)
        {
            return Unit.Value;
        }
        foreach (var changedFile in commit.CommitFiles)
        {
            if (!GLobMatcher.IsMatch(changedFile.Path, feature.GlobPatterns))
            {
                feature.GlobPatterns.Add(changedFile.Path);
                changed = true;
            }
        }
        if (changed)
        {
            await _architectureManager.UpdateFeatureAsync(request.Principal, feature);
        }

        // Update commit to reference the feeature
        commit.FeatureNames ??= [];
        if (!commit.FeatureNames.Contains(feature.Name))
        {
            commit.FeatureNames.Add(feature.Name);
            await _commitManager.UpdateCommitAsync(request.Principal, commit);
        }
        return Unit.Value;
    }
}
