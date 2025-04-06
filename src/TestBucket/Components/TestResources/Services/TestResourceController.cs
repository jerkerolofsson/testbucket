
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Components.TestResources.Services;

internal class TestResourceController : TenantBaseService
{
    private readonly ITestResourceManager _manager;

    public TestResourceController(
        ITestResourceManager manager,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task<PagedResult<TestResource>> GetResourcesAsync(int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.BrowseAsync(principal, offset, count);
    }
}
