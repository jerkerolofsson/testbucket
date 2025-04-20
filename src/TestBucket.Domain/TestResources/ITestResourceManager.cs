using System.Security.Claims;

using TestBucket.Contracts.TestResources;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources;
public interface ITestResourceManager
{
    /// <summary>
    /// Adds a test resource
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task AddAsync(ClaimsPrincipal principal, TestResource resource);

    /// <summary>
    /// Browses for test resources
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<TestResource>> BrowseAsync(ClaimsPrincipal principal, int offset, int count);

    /// <summary>
    /// Deletes a test resource
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task DeleteAsync(ClaimsPrincipal principal, TestResource resource);

    /// <summary>
    /// Updates a test resource
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    Task UpdateAsync(ClaimsPrincipal principal, TestResource resource);

    /// <summary>
    /// Updates the state for a collection of resources received from a remote resource server
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="resources"></param>
    /// <returns></returns>
    Task UpdateResourcesFromResourceServerAsync(ClaimsPrincipal principal, List<TestResourceDto> resources);
}