using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Fields;

namespace TestBucket.Domain.Labels.DataSources;
internal class LabelDataSource : IFieldCompletionsProvider
{
    private readonly ILabelManager _manager;

    public LabelDataSource(ILabelManager manager)
    {
        _manager = manager;
    }

    public async Task<IReadOnlyList<GenericVisualEntity>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken)
    {
        if(type == FieldDataSourceType.Labels)
        {
            var labels = await _manager.GetLabelsAsync(principal, projectId);
            return labels.Where(x=>x.Title != null).Select(x=> new GenericVisualEntity { Title = x.Title, Description = x.Description, Color = x.Color }).ToList();
        }
        return [];
    }

    public async Task<IReadOnlyList<GenericVisualEntity>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken)
    {
        if (type == FieldDataSourceType.Labels)
        {
            var labels = await _manager.SearchLabelsAsync(principal, projectId, text, offset:0, count);
            return labels.Where(x => x.Title != null).Select(x => new GenericVisualEntity { Title = x.Title, Description = x.Description, Color = x.Color }).ToList();
        }
        return [];
    }
}
