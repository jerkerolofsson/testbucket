using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Data.Sequence;

/// <summary>
/// Generates sequence numbers
/// </summary>
internal class SequenceGenerator : ISequenceGenerator
{
    /// <summary>
    /// Next number
    /// </summary>
    private readonly SemaphoreSlim _lock = new(1);
    private readonly ConcurrentDictionary<string, int> _max = [];

    /// <summary>
    /// Generates a new sequence number. 
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <param name="entityType">Only used for caching the correct. Should correspond to getLastUsedFunc</param>
    /// <param name="getLastUsedFunc">Returns the latest sequence number used or zero if there are no matches</param>
    /// <returns></returns>
    public async ValueTask<int> GenerateSequenceNumberAsync(string tenantId, long projectId, string entityType, Func<string,long,CancellationToken, ValueTask<int>> getLastUsedFunc, CancellationToken cancellationToken)
    {
        var key = tenantId + projectId + entityType;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (!_max.TryGetValue(key, out var next))
            {
                next = await getLastUsedFunc(tenantId, projectId, cancellationToken);
            }
            next++;
            _max[key] = next;
            return next;
        }
        finally
        {
            _lock.Release();
        }
    }
}
