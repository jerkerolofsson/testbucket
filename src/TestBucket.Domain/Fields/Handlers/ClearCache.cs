using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields.Handlers;
public record class ClearFieldCacheRequest(string TenantId, FieldDataSourceType DataSourceType) : IRequest;

public class ClearFieldCacheHandler : IRequestHandler<ClearFieldCacheRequest>
{
    private readonly IFieldDefinitionManager _manager;
    public ClearFieldCacheHandler(IFieldDefinitionManager completionsProvider)
    {
        _manager = completionsProvider;
    }
    public async ValueTask<Unit> Handle(ClearFieldCacheRequest request, CancellationToken cancellationToken)
    {
        // Clear the cache for the specified data source type
        await _manager.ClearCacheAsync(request.TenantId, request.DataSourceType);

        return Unit.Value;
    }
}
