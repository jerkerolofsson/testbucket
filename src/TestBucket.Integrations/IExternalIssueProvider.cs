using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Integrations;

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
    /// <param name="system"></param>
    /// <param name="text"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueDto>> SearchAsync(ExternalSystemDto system, string? text, int offset, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns information about an issue
    /// </summary>
    /// <param name="system"></param>
    /// <param name="externalIssueId"></param>
    /// <returns></returns>
    Task<IssueDto?> GetIssueAsync(ExternalSystemDto system, string externalIssueId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all issues modified/updated between the specified dates
    /// </summary>
    /// <param name="system"></param>
    /// <param name="from"></param>
    /// <param name="until"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueDto>> GetIssuesAsync(ExternalSystemDto system, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new issue in an external system.
    /// </summary>
    /// <param name="externalSystemDto"></param>
    /// <param name="issueDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateIssueAsync(IExtensionApi api, ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken);

    /// <summary>
    /// Saves changes to an issue in an external system.
    /// </summary>
    /// <param name="externalSystemDto"></param>
    /// <param name="issueDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateIssueAsync(IExtensionApi api, ExternalSystemDto externalSystemDto, IssueDto issueDto, CancellationToken cancellationToken);
}
