
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Data.Sequence;

internal interface ISequenceGenerator
{
    ValueTask<int> GenerateSequenceNumberAsync(string tenantId, long projectId, string entityType, Func<string, long, CancellationToken, ValueTask<int>> getLastUsedFunc, CancellationToken cancellationToken);
}