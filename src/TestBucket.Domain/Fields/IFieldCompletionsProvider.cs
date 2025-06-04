using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.Fields;
public interface IFieldCompletionsProvider
{
    /// <summary>
    /// Searches for options (auto complete)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="type"></param>
    /// <param name="projectId"></param>
    /// <param name="text"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<GenericVisualEntity>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, string text, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all options (single selection, multi selection)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="type"></param>
    /// <param name="projectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<GenericVisualEntity>> GetOptionsAsync(ClaimsPrincipal principal, FieldDataSourceType type, long projectId, CancellationToken cancellationToken);
}
