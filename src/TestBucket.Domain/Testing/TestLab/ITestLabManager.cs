using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.Testing.TestLab;
public interface ITestLabManager
{
    /// <summary>
    /// Adds an observer that will receive notifications when things are changing
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ITestLabObserver observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ITestLabObserver observer);


    Task AddFolderAsync(ClaimsPrincipal principal, TestLabFolder folder);
    Task DeleteFolderAsync(ClaimsPrincipal principal, long folderId);
    Task<IReadOnlyList<TestLabFolder>> GetFoldersAsync(ClaimsPrincipal principal, long projectId, long parentId);
    Task<IReadOnlyList<TestLabFolder>> GetRootFoldersAsync(ClaimsPrincipal principal, long projectId);
    Task UpdateFolderAsync(ClaimsPrincipal principal, TestLabFolder testLabFolder);
}
