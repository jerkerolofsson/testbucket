using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
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

    public async Task<IReadOnlyList<GenericVisualEntity>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken)
    {
        if(type == FieldDataSourceType.Features)
        {
            var items = await _manager.GetFeaturesAsync(principal, projectId);
            return items.Where(x => x.Name != null).Select(x => new GenericVisualEntity { Embedding = x.Embedding?.Memory, Title = x.Name, Description = x.Description }).ToList();
        }
        return [];
    }

    public async Task<IReadOnlyList<GenericVisualEntity>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken)
    {
        if (type == FieldDataSourceType.Features)
        {
            var result = await _manager.SearchFeaturesAsync(principal, projectId, text, offset:0, count);
            return result.Items.Where(x => x.Name != null && x.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase)).Select(x => new GenericVisualEntity { Embedding = x.Embedding?.Memory, Title = x.Name, Description = x.Description }).ToList();
        }
        return [];
    }
}
