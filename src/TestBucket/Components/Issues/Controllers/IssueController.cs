using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Components.Issues.Controllers;

internal class IssueController : TenantBaseService
{
    private readonly IIssueManager _manager;

    public IssueController(IIssueManager manager, AuthenticationStateProvider provider)
        : base(provider)
    {
        _manager = manager;
    }

    public async Task AddIssueAsync(LinkedIssue issue)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddLinkedIssueAsync(principal, issue);
    }
}
