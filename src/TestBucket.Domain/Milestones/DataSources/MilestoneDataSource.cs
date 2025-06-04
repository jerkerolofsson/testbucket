using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Fields;

namespace TestBucket.Domain.Milestones.DataSources;
internal class MilestoneDataSource : IFieldCompletionsProvider
{
    private readonly IMilestoneManager _manager;

    public MilestoneDataSource(IMilestoneManager manager)
    {
        _manager = manager;
    }

    public async Task<IReadOnlyList<GenericVisualEntity>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken)
    {
        if(type == FieldDataSourceType.Milestones)
        {
            var milestones = await _manager.GetMilestonesAsync(principal, projectId);
            return milestones.Where(x => x.Title != null).Select(x => new GenericVisualEntity { Title = x.Title, Description = x.Description }).ToList();
        }
        return [];
    }

    public async Task<IReadOnlyList<GenericVisualEntity>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken)
    {
        if (type == FieldDataSourceType.Milestones)
        {
            var milestones = await _manager.SearchMilestonesAsync(principal, projectId, text, offset:0, count);
            return milestones.Where(x => x.Title != null && x.Title.Contains(text, StringComparison.InvariantCultureIgnoreCase)).Select(x => new GenericVisualEntity { Title = x.Title, Description = x.Description }).ToList();
        }
        return [];
    }
}
