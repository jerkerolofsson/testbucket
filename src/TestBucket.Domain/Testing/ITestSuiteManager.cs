using System.Security.Claims;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;
public interface ITestSuiteManager
{
    void AddObserver(ITestSuiteObserver observer);
    void RemoveObserver(ITestSuiteObserver observer);


    void AddFolderObserver(ITestSuiteFolderObserver observer);
    void RemoveFolderObserver(ITestSuiteFolderObserver observer);
    Task<TestSuiteFolder> AddTestSuiteFolderAsync(ClaimsPrincipal principal, long? projectId, long testSuiteId, long? parentFolderId, string name);
    Task DeleteTestSuiteFolderAsync(ClaimsPrincipal principal, TestSuiteFolder folder);
    Task SaveTestSuiteFolderAsync(ClaimsPrincipal principal, TestSuiteFolder folder);

    /// <summary>
    /// Deletes a test suite folder by id
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="folderId"></param>
    /// <returns></returns>
    Task DeleteTestSuiteFolderByIdAsync(ClaimsPrincipal principal, long folderId);
    Task<TestSuite> AddTestSuiteAsync(ClaimsPrincipal principal, long? teamId, long? projectId, string name, string? ciCdSystem = null, string? ciCdRef = null);
    Task<TestSuite?> GetTestSuiteByIdAsync(ClaimsPrincipal principal, long id);
    Task UpdateTestSuiteAsync(ClaimsPrincipal principal, TestSuite suite);
    Task DeleteTestSuiteByIdAsync(ClaimsPrincipal principal, long testSuiteId);
    Task<TestSuite?> GetTestSuiteByNameAsync(ClaimsPrincipal principal, long? teamId, long? projectId, string suiteName);
    Task<PagedResult<TestSuite>> SearchTestSuitesAsync(ClaimsPrincipal principal, SearchQuery query);
    Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(ClaimsPrincipal principal, long? projectId, long testSuiteId, long? parentFolderId);
}