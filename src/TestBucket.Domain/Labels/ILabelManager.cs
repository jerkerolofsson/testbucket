using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Labels.Models;

namespace TestBucket.Domain.Labels;

/// <summary>
/// Provides methods for managing project labe;s, including retrieval, creation, updating, searching, and deletion.
/// </summary>
public interface ILabelManager
{
    /// <summary>
    /// Returns a label by its name within a specific project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="projectId">The ID of the project to search in.</param>
    /// <param name="name">The name of the label to retrieve.</param>
    /// <returns>The label if found; otherwise, <c>null</c>.</returns>
    Task<Label?> GetLabelByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    /// <summary>
    /// Adds a new label to a project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="label">The label to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLabelAsync(ClaimsPrincipal principal, Label label);

    /// <summary>
    /// Retrieves all labels for a specific project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A read-only list of labels.</returns>
    Task<IReadOnlyList<Label>> GetLabelsAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Updates an existing label.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="label">The label to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateLabelAsync(ClaimsPrincipal principal, Label label);

    /// <summary>
    /// Searches for labels in a project matching the specified text.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="projectId">The ID of the project to search in.</param>
    /// <param name="text">The search text to filter labels.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="count">The maximum number of items to return.</param>
    /// <returns>A read-only list of matching labels.</returns>
    Task<IReadOnlyList<Label>> SearchLabelsAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count);

    /// <summary>
    /// Deletes a label from a project.
    /// </summary>
    /// <param name="principal">The user principal performing the operation.</param>
    /// <param name="label">The label to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(ClaimsPrincipal principal, Label label);
}