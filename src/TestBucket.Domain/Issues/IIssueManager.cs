using System.Security.Claims;

using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues;
public interface IIssueManager
{
    Task AddLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue);
    Task DeleteLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue);
}