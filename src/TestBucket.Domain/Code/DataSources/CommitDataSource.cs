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
internal class CommitDataSource : IFieldCompletionsProvider
{
    private readonly ICommitManager _manager;

    public CommitDataSource(ICommitManager manager)
    {
        _manager = manager;
    }

    public Task<IReadOnlyList<GenericVisualEntity>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken)
    {
        if(type == FieldDataSourceType.Commit)
        {
            //var features = await _manager.GetComponentsAsync(principal, projectId);
            //return features.Select(x=>x.Name).ToList();
        }
        return Task.FromResult<IReadOnlyList<GenericVisualEntity>>([]);
    }

    public async Task<IReadOnlyList<GenericVisualEntity>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken)
    {
        if (type == FieldDataSourceType.Commit)
        {
            var result = await _manager.SearchCommitsAsync(principal, projectId, text, offset:0, count);
            return result.Items.Select(x => new GenericVisualEntity { Title = x.Sha, Description = x.ShortDescription }).ToList();
        }
        return [];
    }
}
