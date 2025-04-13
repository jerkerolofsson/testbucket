using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.Models;

namespace TestBucket.Contracts.Integrations;

/// <summary>
/// Implemented by an extension
/// </summary>
public interface IExternalIssueProvider
{
    string SystemName { get; }

    /// <summary>
    /// Searches for issues
    /// </summary>
    /// <param name="text"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto config, string text, int offset, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns information about an issue
    /// </summary>
    /// <param name="config"></param>
    /// <param name="externalIssueId"></param>
    /// <returns></returns>
    Task<IssueDto?> GetIssueAsync(ExternalSystemDto config, string externalIssueId, CancellationToken cancellationToken);
}
