using System.Security.Claims;

using TestBucket.Contracts.TestResources;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources;
public interface ITestResourceManager
{
    Task AddAsync(ClaimsPrincipal principal, TestResource resource);
    Task<PagedResult<TestResource>> BrowseAsync(ClaimsPrincipal principal, int offset, int count);
    Task DeleteAsync(ClaimsPrincipal principal, TestResource resource);
    Task UpdateAsync(ClaimsPrincipal principal, TestResource resource);
    Task UpdateResourcesFromResourceServerAsync(ClaimsPrincipal principal, List<TestResourceDto> resources);
}