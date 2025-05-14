using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;

namespace TestBucket.Components.Issues.Controllers;

internal class IssueController : TenantBaseService
{
    private readonly IIssueManager _manager;

    public IssueController(IIssueManager manager, AuthenticationStateProvider provider)
        : base(provider)
    {
        _manager = manager;
    }

    public async Task<PagedResult<LocalIssue>> SearchAsync(long projectId, string text, int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.SearchLocalIssuesAsync(SearchIssueRequestParser.Parse(principal, projectId, text), offset, count);
    }

    public async Task<LocalIssue?> GetIssueByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetIssueByIdAsync(principal, id);
    }
    public async Task UpdateIssueAsync(LocalIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateLocalIssueAsync(principal, issue);
    }
    public async Task AddIssueAsync(LocalIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddLocalIssueAsync(principal, issue);
    }
    public async Task AddIssueAsync(LinkedIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddLinkedIssueAsync(principal, issue);
    }
}
