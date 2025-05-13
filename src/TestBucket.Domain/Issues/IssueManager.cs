using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

using Mediator;

using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Issues.Specifications;
using TestBucket.Domain.Shared.Specifications;
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

    #region Local Issues
    public async Task AddLocalIssueAsync(ClaimsPrincipal principal, LocalIssue issue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.ReadWrite);
        issue.CreatedBy ??= principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.ModifiedBy ??= principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.TenantId = principal.GetTenantIdOrThrow();
        await _repository.AddLocalIssueAsync(issue);
    }


    public async Task<PagedResult<LocalIssue>> SearchLocalIssuesAsync(SearchIssueRequest request, int offset, int count)
    {
        var principal = request.Principal;
        List<FilterSpecification<LocalIssue>> filters = [
            new FilterByProject<LocalIssue>(request.ProjectId),
            new FilterByTenant<LocalIssue>(principal.GetTenantIdOrThrow())
        ];
        if (!string.IsNullOrEmpty(request.Text))
        {
            filters.Add(new FindLocalIssueByText(request.Text));
        }
        if (!string.IsNullOrEmpty(request.ExternalSystemName))
        {
            filters.Add(new FindLocalIssueByExternalSystemName(request.ExternalSystemName));
        }
        if (request.ExternalSystemId != null)
        {
            filters.Add(new FindLocalIssueByExternalSystemId(request.ExternalSystemId.Value));
        }

        return await _repository.SearchAsync(filters, offset, count);
    }
    public async Task<LocalIssue?> FindLocalIssueFromExternalAsync(ClaimsPrincipal principal, long testProjectId, long? externalSystemId, string? externalId)
    {

        List<FilterSpecification<LocalIssue>> filters = [
            new FilterByProject<LocalIssue>(testProjectId),
            new FilterByTenant<LocalIssue>(principal.GetTenantIdOrThrow())
        ];
        if (externalSystemId is not null)
        {
            filters.Add(new FindLocalIssueByExternalSystemId(externalSystemId.Value));
        }
        if (externalId is not null)
        {
            filters.Add(new FindLocalIssueByExternalId(externalId));
        }

        var result = await _repository.SearchAsync(filters, 0, 1);
        return result.Items.FirstOrDefault();
    }

    public async Task UpdateLocalIssueAsync(ClaimsPrincipal principal, LocalIssue existingIssue)
    {
        principal.ThrowIfEntityTenantIsDifferent(existingIssue);
        await _repository.UpdateLocalIssueAsync(existingIssue);
    }
    #endregion Local Issues

    #region Linked Issues

    public async Task<PagedResult<LinkedIssue>> SearchLinkedIssuesAsync(SearchIssueRequest request, int offset, int count)
    {
        var principal = request.Principal;
        List<FilterSpecification<LinkedIssue>> filters = [
            new FilterByProject<LinkedIssue>(request.ProjectId),
            new FilterByTenant<LinkedIssue>(principal.GetTenantIdOrThrow())
        ];

        if (request.Text is not null)
        {
            filters.Add(new FindLinkedIssueByText(request.Text));
        }

        return await _repository.SearchAsync(filters, offset, count);
    }

    /// <summary>
    /// Refreshes a linked issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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
                IssueMapper.CopyTo(response.Issue, issue);
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

    #endregion Linked Issues
}
