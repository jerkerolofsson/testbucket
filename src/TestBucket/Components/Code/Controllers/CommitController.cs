
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Code.Yaml;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Code.Models;

namespace TestBucket.Components.Code.Controllers;

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
