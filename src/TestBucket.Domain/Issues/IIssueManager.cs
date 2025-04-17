using System.Security.Claims;

using TestBucket.Domain.Issues.Models;

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
}