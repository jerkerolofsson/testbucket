
using TestBucket.Domain.Audit;
using TestBucket.Domain.Audit.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Components.Shared.Review.Controllers;

internal class ReviewController : TenantBaseService
{
    private readonly IAuditor _auditor;
    private readonly ITestCaseManager _testCaseManager;

    public ReviewController(AuthenticationStateProvider authenticationStateProvider, IAuditor auditor, ITestCaseManager testCaseManager) : base(authenticationStateProvider)
    {
        _auditor = auditor;
        _testCaseManager = testCaseManager;
    }


    public async Task VoteAsync(TestCase test, int vote)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var user = principal.Identity?.Name;
        if (user is null)
        {
            return;
        }
        test.ReviewAssignedTo ??= [];
        var entry = test.ReviewAssignedTo.FirstOrDefault(x => x.UserName == user);
        if (entry is null)
        {
            entry = new Domain.Shared.AssignedReviewer() { Role = "", UserName = user };
            test.ReviewAssignedTo.Add(entry);
        }
        entry.Vote = vote;
        await _testCaseManager.SaveTestCaseAsync(principal, test);
    }

    public async Task<IReadOnlyList<AuditEntry>> GetTestCaseDiffAsync(long testCaseId)
    {
        ClaimsPrincipal principal = await GetUserClaimsPrincipalAsync();
        return await _auditor.GetEntriesAsync<TestCase>(principal,testCaseId);
    }
}
