using TestBucket.Domain.Search;
using TestBucket.Domain.Search.Models;

namespace TestBucket.Components.Search.Controllers;

internal class UnifiedSearchController : TenantBaseService
{
    private readonly IUnifiedSearchManager _manager;

    public UnifiedSearchController(AuthenticationStateProvider authenticationStateProvider, IUnifiedSearchManager manager)
        : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task<List<SearchResult>> SearchAsync(TestProject? testProject, string text, CancellationToken cancellationToken)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.SearchAsync(principal, testProject, text, cancellationToken);
    }

}