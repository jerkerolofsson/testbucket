using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRepository;

/// <summary>
/// Provides methods for managing test repository folders, including creation, retrieval, and updates.
/// </summary>
public interface ITestRepositoryManager
{
    /// <summary>
    /// Adds an observer that will receive notifications when things are changing
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ITestRepositoryObserver observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ITestRepositoryObserver observer);


    /// <summary>
    /// Deletes a folder
    /// </summary>
    /// <param name="principal">The user performing the operation.</param>
    /// <param name="folder">The folder to add.</param>
    /// <returns></returns>
    Task DeleteFolderAsync(ClaimsPrincipal principal, long folderId);

    /// <summary>
    /// Adds a new folder to the test repository.
    /// </summary>
    /// <param name="principal">The user performing the operation.</param>
    /// <param name="folder">The folder to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddFolderAsync(ClaimsPrincipal principal, TestRepositoryFolder folder);

    /// <summary>
    /// Retrieves the folders under a specified parent folder within a project.
    /// </summary>
    /// <param name="principal">The user performing the operation.</param>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="parentId">The ID of the parent folder.</param>
    /// <returns>A read-only list of child folders.</returns>
    Task<IReadOnlyList<TestRepositoryFolder>> GetFoldersAsync(ClaimsPrincipal principal, long projectId, long parentId);

    /// <summary>
    /// Retrieves the root folders for a given project.
    /// </summary>
    /// <param name="principal">The user performing the operation.</param>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A read-only list of root folders.</returns>
    Task<IReadOnlyList<TestRepositoryFolder>> GetRootFoldersAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Updates an existing folder in the test repository.
    /// </summary>
    /// <param name="principal">The user performing the operation.</param>
    /// <param name="testRepositoryFolder">The folder with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateFolderAsync(ClaimsPrincipal principal, TestRepositoryFolder testRepositoryFolder);
    Task<TestRepositoryFolder?> GetFolderByIdAsync(ClaimsPrincipal principal, long repositoryFolderId);
}