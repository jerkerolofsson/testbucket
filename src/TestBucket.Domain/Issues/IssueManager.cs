using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

using Mediator;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Testing;

namespace TestBucket.Domain.Issues;

public class IssueManager : IIssueManager
{
    private readonly IIssueRepository _repository;
    private readonly IMediator _mediator;
    private readonly List<ILinkedIssueObserver> _observers = new();

    public IssueManager(IIssueRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(ILinkedIssueObserver observer) => _observers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(ILinkedIssueObserver observer) => _observers.Remove(observer);

    public async Task RefreshLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.ReadWrite);
        issue.CreatedBy = principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.ModifiedBy = principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.TenantId = principal.GetTenantIdOrThrow();

        if(issue.ExternalId is not null && issue.TestProjectId is not null)
        {
            var response = await _mediator.Send(new RefreshIssueRequest(principal, issue.TestProjectId.Value, issue.ExternalId));
            if (response.Issue is not null)
            {
                issue.Title = response.Issue.Title;
                issue.ExternalDisplayId = response.Issue.ExternalDisplayId;
                issue.State = response.Issue.State;
                issue.Description = response.Issue.Description;
                await _repository.UpdateLinkedIssueAsync(issue);
            }

            foreach (var observer in _observers)
            {
                await observer.OnLinkedIssueAddedAsync(issue);
            }
        }
    }
    public async Task AddLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.ReadWrite);
        issue.CreatedBy = principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.ModifiedBy = principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.TenantId = principal.GetTenantIdOrThrow();
        await _repository.AddLinkedIssueAsync(issue);

        foreach (var observer in _observers)
        {
            await observer.OnLinkedIssueAddedAsync(issue);
        }
    }

    public async Task DeleteLinkedIssueAsync(ClaimsPrincipal principal, LinkedIssue issue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.ReadWrite);
        principal.ThrowIfEntityTenantIsDifferent(issue);

        await _repository.DeleteLinkedIssueAsync(issue.Id);

        foreach(var observer in _observers)
        {
            await observer.OnLinkedIssueDeletedAsync(issue);
        }
    }
}
