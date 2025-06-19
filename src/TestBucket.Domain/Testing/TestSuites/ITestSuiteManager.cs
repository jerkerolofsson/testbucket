using System.Runtime.CompilerServices;
using System.Security.Claims;

using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites.Search;

namespace TestBucket.Domain.Testing.TestSuites;
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
    Task<TestSuite> AddTestSuiteAsync(ClaimsPrincipal principal, TestSuite testSuite);
    Task<TestSuite> AddTestSuiteAsync(ClaimsPrincipal principal, long? teamId, long? projectId, string name, string? ciCdSystem = null, string? ciCdRef = null);
    Task<TestSuite?> GetTestSuiteByIdAsync(ClaimsPrincipal principal, long id);
    Task UpdateTestSuiteAsync(ClaimsPrincipal principal, TestSuite suite);
    Task DeleteTestSuiteByIdAsync(ClaimsPrincipal principal, long testSuiteId);
    Task<TestSuite?> GetTestSuiteBySlugAsync(ClaimsPrincipal principal, long? projectId, string slug);
    Task<TestSuite?> GetTestSuiteByNameAsync(ClaimsPrincipal principal, long? teamId, long? projectId, string suiteName);

    /// <summary>
    /// Searches for test suites
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<TestSuite>> SearchTestSuitesAsync(ClaimsPrincipal principal, SearchTestSuiteQuery query);
    Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(ClaimsPrincipal principal, long? projectId, long testSuiteId, long? parentFolderId);


    /// <summary>
    /// Enumerates all items by fetching page-by-page
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<TestSuite> EnumerateAsync(ClaimsPrincipal principal, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageSize = 20;
        var offset = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await SearchTestSuitesAsync(principal, new SearchTestSuiteQuery() { Offset = offset, Count = pageSize });
            foreach (var item in result.Items)
            {
                yield return item;
            }
            if (result.Items.Length != pageSize)
            {
                break;
            }
            offset += result.Items.Length;
        }
    }
}