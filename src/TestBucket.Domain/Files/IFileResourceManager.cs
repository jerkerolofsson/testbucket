
using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;
public interface IFileResourceManager
{
    void AddObserver(IFileResourceObserver observer);

    /// <summary>
    /// Adds a file resource
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task AddResourceAsync(ClaimsPrincipal principal, FileResource resource);
    Task<bool> DeleteResourceByIdAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<FileResource>> GetRequirementAttachmentsAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<FileResource>> GetRequirementSpecificationAttachmentsAsync(ClaimsPrincipal principal, long id);
    Task<FileResource?> GetResourceByIdAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<FileResource>> GetTestCaseAttachmentsAsync(ClaimsPrincipal principal, long testCaseId);
    Task<IReadOnlyList<FileResource>> GetTestCaseRunAttachmentsAsync(ClaimsPrincipal principal, long testCaseRunId);
    Task<IReadOnlyList<FileResource>> GetTestProjectAttachmentsAsync(ClaimsPrincipal principal, long testProjectId);
    Task<IReadOnlyList<FileResource>> GetTestRunAttachmentsAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<FileResource>> GetTestSuiteAttachmentsAsync(ClaimsPrincipal principal, long testSuiteId);
    Task<IReadOnlyList<FileResource>> GetTestSuiteFolderAttachmentsAsync(ClaimsPrincipal principal, long testSuiteFolderId);
    void RemoveObserver(IFileResourceObserver observer);
}