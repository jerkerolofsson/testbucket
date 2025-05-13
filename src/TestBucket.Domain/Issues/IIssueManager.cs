using System.Security.Claims;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;

namespace TestBucket.Domain.Issues;
public interface IIssueManager
{
    /// <summary>
    /// Adds a linked issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task AddLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue);


    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ILinkedIssueObserver observer);

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ILinkedIssueObserver observer);

    /// <summary>
    /// Removes a linked issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task DeleteLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue);

    /// <summary>
    /// Reloads information from an external source
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task RefreshLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue);
    Task<PagedResult<LinkedIssue>> SearchLinkedIssuesAsync(SearchIssueRequest request, int offset, int count);

    #region Local issues
    Task AddLocalIssueAsync(ClaimsPrincipal principal, LocalIssue issue);

    Task<PagedResult<LocalIssue>> SearchLocalIssuesAsync(SearchIssueRequest request, int offset, int count);
    Task<LocalIssue?> FindLocalIssueFromExternalAsync(ClaimsPrincipal principal, long testProjectId, long? externalSystemId, string? externalId);
    Task UpdateLocalIssueAsync(ClaimsPrincipal principal, LocalIssue existingIssue);

    #endregion
}