using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Fields;

namespace TestBucket.Domain.Code.DataSources;
internal class FeatureDataSource : IFieldCompletionsProvider
{
    private readonly IArchitectureManager _manager;

    public FeatureDataSource(IArchitectureManager manager)
    {
        _manager = manager;
    }

    public async Task<IReadOnlyList<string>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken)
    {
        if(type == FieldDataSourceType.Features)
        {
            var features = await _manager.GetFeaturesAsync(principal, projectId);
            return features.Select(x=>x.Name).ToList();
        }
        return [];
    }

    public async Task<IReadOnlyList<string>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken)
    {
        if (type == FieldDataSourceType.Features)
        {
            var features = await _manager.SearchFeaturesAsync(principal, projectId, text, offset:0, count);
            return features.Select(x => x.Name).ToList();
        }
        return [];
    }
}
