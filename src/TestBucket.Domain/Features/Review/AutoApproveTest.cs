using Mediator;

using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.Testing.Events;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.Features.Review;
public class AutoApproveTest : INotificationHandler<TestCaseSavingEvent>
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly ITestCaseManager _testCaseManager;

    public AutoApproveTest(ISettingsProvider settingsProvider, ITestCaseManager testCaseManager)
    {
        _settingsProvider = settingsProvider;
        _testCaseManager = testCaseManager;
    }

    public async ValueTask Handle(TestCaseSavingEvent notification, CancellationToken cancellationToken)
    {
        var test = notification.New;
        var result = await DidEveryoneVotePositiveAsync(notification.Principal, test);
        if(result == true)
        {
            await _testCaseManager.ApproveTestAsync(notification.Principal, test);
        }
    }

    internal async Task<bool> DidEveryoneVotePositiveAsync(ClaimsPrincipal principal, TestCase test)
    {
        if (test.TestProjectId is null || test.TenantId is null)
        {
            return false;
        }
        if (test.MappedState != Contracts.Testing.States.MappedTestState.Review)
        {
            return false;
        }
        if (test.ReviewAssignedTo is null)
        {
            return false;
        }
        if (test.ReviewAssignedTo.Count == 0)
        {
            return false;
        }


        // Read settings
        var settings = await _settingsProvider.GetDomainSettingsAsync<ReviewSettings>(test.TenantId!, test.TestProjectId!.Value);
        var enabled = settings?.AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes == true;
        if (!enabled)
        {
            return false;
        }

        var countPositive = test.ReviewAssignedTo.Where(x => x.Vote > 0).Count();
        if (countPositive == test.ReviewAssignedTo.Count)
        {
            // All reviewers voted positive
            return true;
        }
        return false;
    }
}
