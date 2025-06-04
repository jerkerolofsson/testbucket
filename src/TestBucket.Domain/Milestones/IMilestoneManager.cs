using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Milestones;

/// <summary>
/// Provides methods for managing project milestones, including retrieval, creation, updating, searching, and deletion.
/// </summary>
public interface IMilestoneManager
{
    /// <summary>
    /// Returns a milestone by its name within a specific project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="projectId">The ID of the project to search in.</param>
    /// <param name="name">The name of the milestone to retrieve.</param>
    /// <returns>The milestone if found; otherwise, <c>null</c>.</returns>
    Task<Milestone?> GetMilestoneByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    /// <summary>
    /// Adds a new milestone to a project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="milestone">The milestone to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddMilestoneAsync(ClaimsPrincipal principal, Milestone milestone);

    /// <summary>
    /// Retrieves all milestones for a specific project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A read-only list of milestones.</returns>
    Task<IReadOnlyList<Milestone>> GetMilestonesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Updates an existing milestone.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="milestone">The milestone to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMilestoneAsync(ClaimsPrincipal principal, Milestone milestone);

    /// <summary>
    /// Searches for milestones in a project matching the specified text.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="projectId">The ID of the project to search in.</param>
    /// <param name="text">The search text to filter milestones.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="count">The maximum number of items to return.</param>
    /// <returns>A read-only list of matching milestones.</returns>
    Task<IReadOnlyList<Milestone>> SearchMilestonesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);

    /// <summary>
    /// Deletes a milestone from a project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="milestone">The milestone to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(ClaimsPrincipal principal, Milestone milestone);
}