using System.Security.Claims;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues;

public class IssueManager : IIssueManager
{
    private readonly IIssueRepository _repository;

    public IssueManager(IIssueRepository repository)
    {
        _repository = repository;
    }

    public async Task AddLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue)
    {
        issue.CreatedBy = principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.ModifiedBy = principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.TenantId = principal.GetTenantIdOrThrow();
        await _repository.AddLinkedIssueAsync(issue);
    }

    public async Task DeleteLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue)
    {
        principal.ThrowIfEntityTenantIsDifferent(issue);
        await _repository.DeleteLinkedIssueAsync(issue.Id);
    }
}
