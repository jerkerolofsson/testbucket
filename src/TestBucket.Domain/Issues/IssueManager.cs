using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues.Events;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Issues.Specifications;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Issues;

public class IssueManager : IIssueManager
{
    private readonly IIssueRepository _repository;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IMediator _mediator;
    private readonly List<ILinkedIssueObserver> _observers = new();

    public IssueManager(IIssueRepository repository, IMediator mediator, IFieldDefinitionManager fieldDefinitionManager)
    {
        _repository = repository;
        _mediator = mediator;
        _fieldDefinitionManager = fieldDefinitionManager;
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

    /// <summary>
    /// Returns number of issues per state, matching the filter
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Dictionary<MappedIssueState, int>> GetIssueCountPerStateAsync(ClaimsPrincipal principal, SearchIssueQuery request)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        List<FilterSpecification<LocalIssue>> filters = [
            new FilterByTenant<LocalIssue>(principal.GetTenantIdOrThrow())
        ];

        filters.AddRange(SearchIssueRequestBuilder.Build(request));
        return await _repository.GetIssueCountPerStateAsync(filters);
    }

    public async Task AddLocalIssueAsync(ClaimsPrincipal principal, LocalIssue issue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Write);

        if (issue.State == null)
        {
            issue.State = IssueStates.Open;
            issue.MappedState = MappedIssueState.Open;
        }
        if (issue.IssueType == null)
        {
            issue.IssueType = IssueTypes.Issue;
            issue.MappedType = MappedIssueType.Issue;
        }

        issue.CreatedBy ??= principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.Author ??= issue.CreatedBy;
        issue.ModifiedBy ??= principal.Identity?.Name ?? throw new ArgumentException("No user name in principal");
        issue.TenantId = principal.GetTenantIdOrThrow();
        await _repository.AddLocalIssueAsync(issue);
    }

    public async Task<PagedResult<LocalIssue>> SearchLocalIssuesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count)
    {
        var definitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, projectId, FieldTarget.Issue);
        return await SearchLocalIssuesAsync(principal, SearchIssueRequestParser.Parse(projectId, text, definitions), offset, count);
    }

    public async Task<PagedResult<LocalIssue>> SearchLocalIssuesAsync(ClaimsPrincipal principal, SearchIssueQuery request, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        List<FilterSpecification<LocalIssue>> filters = [
            new FilterByTenant<LocalIssue>(principal.GetTenantIdOrThrow())
        ];

        filters.AddRange(SearchIssueRequestBuilder.Build(request));
        return await _repository.SearchAsync(filters, offset, count);
    }
    public async Task<LocalIssue?> FindLocalIssueFromExternalAsync(ClaimsPrincipal principal, long testProjectId, long? externalSystemId, string? externalId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
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

    public async Task UpdateLocalIssueAsync(ClaimsPrincipal principal, LocalIssue updatedIssue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(updatedIssue);

        // Get the existing issue to detect differences
        var existingIssue = await GetIssueByIdAsync(principal, updatedIssue.Id);

        // Save it
        await _repository.UpdateLocalIssueAsync(updatedIssue);

        if (existingIssue is not null && existingIssue.State != updatedIssue.State)
        {
            // State has changed, raise event 
            await _mediator.Publish(new IssueStateChangedNotification(principal, updatedIssue, existingIssue.State));
        }
        if (existingIssue is not null && existingIssue.AssignedTo != updatedIssue.AssignedTo)
        {
            // State has changed, raise event 
            await _mediator.Publish(new IssueAssignmentChanged(principal, updatedIssue, existingIssue.AssignedTo));
        }
    }

    public async Task<LocalIssue?> GetIssueByIdAsync(ClaimsPrincipal principal, long id)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);

        List<FilterSpecification<LocalIssue>> filters = [
            new FindLocalIssueById(id),
            new FilterByTenant<LocalIssue>(principal.GetTenantIdOrThrow())
        ];
        var result = await _repository.SearchAsync(filters, 0, 1);
        return result.Items.FirstOrDefault();
    }
    public async Task DeleteLocalIssueAsync(ClaimsPrincipal principal, LocalIssue issue)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Delete);

        await _repository.DeleteLocalIssueAsync(issue.Id);
    }

    #endregion Local Issues

    #region Linked Issues

    public async Task<PagedResult<LinkedIssue>> SearchLinkedIssuesAsync(ClaimsPrincipal principal, SearchIssueQuery request, int offset, int count)
    {
        List<FilterSpecification<LinkedIssue>> filters = [
            new FilterByTenant<LinkedIssue>(principal.GetTenantIdOrThrow())
        ];
        if(request.ProjectId is not null)
        {
            filters.Add(new FilterByProject<LinkedIssue>(request.ProjectId.Value));
        }

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
