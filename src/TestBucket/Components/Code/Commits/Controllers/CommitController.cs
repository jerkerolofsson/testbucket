
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;

namespace TestBucket.Components.Code.Commits.Controllers;

/// <summary>
/// Blazor controller for commit
/// </summary>
internal class CommitController : TenantBaseService
{
    private readonly ICommitManager _manager;

    public CommitController(AuthenticationStateProvider authenticationStateProvider, ICommitManager manager) : base(authenticationStateProvider)
    {
        _manager = manager;
    }

    public async Task<PagedResult<Commit>> BrowseCommitsAsync(long projectId, int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.BrowseCommitsAsync(principal, projectId, offset, count);
    }

}
