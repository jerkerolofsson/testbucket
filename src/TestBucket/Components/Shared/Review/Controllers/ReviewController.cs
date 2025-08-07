
using TestBucket.Domain.Audit;
using TestBucket.Domain.Audit.Models;

namespace TestBucket.Components.Shared.Review.Controllers;

internal class ReviewController : TenantBaseService
{
    private readonly IAuditor _auditor;

    public ReviewController(AuthenticationStateProvider authenticationStateProvider, IAuditor auditor) : base(authenticationStateProvider)
    {
        _auditor = auditor;
    }

    public async Task<IReadOnlyList<AuditEntry>> GetTestCaseDiffAsync(long testCaseId)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _auditor.GetEntriesAsync<TestCase>(principal,testCaseId);
    }
}
