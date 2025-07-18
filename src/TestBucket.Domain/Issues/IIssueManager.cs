﻿using System.Security.Claims;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Insights.Model;
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

    #region Observer

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ILocalIssueObserver observer);

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ILocalIssueObserver observer);

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

    #endregion Observer

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

    /// <summary>
    /// Searches for linked issues
    /// </summary>
    /// <param name="Principal"></param>
    /// <param name="request"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<LinkedIssue>> SearchLinkedIssuesAsync(ClaimsPrincipal Principal, SearchIssueQuery request, int offset, int count);

    #region Local issues

    /// <summary>
    /// Removes a local issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task DeleteLocalIssueAsync(ClaimsPrincipal principal, LocalIssue issue);

    Task OnLocalIssueFieldChangedAsync(ClaimsPrincipal principal, IssueField field);

    /// <summary>
    /// Finds a local issue by an identifier. Multiple strategies may be attempted to find the issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <param name="issueIdentifier"></param>
    /// <returns></returns>
    Task<LocalIssue?> FindLocalIssueAsync(ClaimsPrincipal principal, long testProjectId, string issueIdentifier);

    Task AddLocalIssueAsync(ClaimsPrincipal principal, LocalIssue issue);

    Task<PagedResult<LocalIssue>> SearchLocalIssuesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);
    Task<PagedResult<LocalIssue>> SearchLocalIssuesAsync(ClaimsPrincipal principal, SearchIssueQuery request, int offset, int count);
    Task<LocalIssue?> FindLocalIssueFromExternalAsync(ClaimsPrincipal principal, long testProjectId, long? externalSystemId, string? externalId);
    Task UpdateLocalIssueAsync(ClaimsPrincipal principal, LocalIssue existingIssue);
    Task<LocalIssue?> GetIssueByIdAsync(ClaimsPrincipal principal, long id);

    #endregion

    #region Insights
    /// <summary>
    /// Returns number of issues, grouped by state
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<InsightsData<MappedIssueState, int>> GetIssueCountPerStateAsync(ClaimsPrincipal principal, SearchIssueQuery request);

    /// <summary>
    /// Returns the number of issues, grouped by the string value of the specified field
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="request"></param>
    /// <param name="fieldDefinitionId"></param>
    /// <returns></returns>
    Task<InsightsData<string, int>> GetIssueCountPerFieldAsync(ClaimsPrincipal principal, SearchIssueQuery request, long fieldDefinitionId);
    Task<InsightsData<DateOnly, int>> GetCreatedIssuesPerDay(ClaimsPrincipal principal, SearchIssueQuery request);
    Task<InsightsData<DateOnly, int>> GetClosedIssuesPerDay(ClaimsPrincipal principal, SearchIssueQuery request);
    Task<PagedResult<LocalIssue>> SemanticSearchLocalIssuesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);
    #endregion
}