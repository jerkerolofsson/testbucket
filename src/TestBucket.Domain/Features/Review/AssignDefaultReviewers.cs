using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Events;

namespace TestBucket.Domain.Features.Review;
public class AssignDefaultReviewers : INotificationHandler<TestCaseSavingEvent>
{
    private readonly ITestCaseRepository _testCaseRepository;

    public AssignDefaultReviewers(ITestCaseRepository testCaseRepository)
    {
        _testCaseRepository = testCaseRepository;
    }

    public static bool IsApplicableRequest(TestCaseSavingEvent notification)
    {
        if (notification.New.TestProjectId is null ||
            notification.New.TenantId is null)
        {
            return false;
        }
        if (notification.Old is null)
        {
            return false;
        }

        // If the state was changed to review
        if (notification.Old.MappedState != MappedTestState.Review && 
            notification.New.MappedState == MappedTestState.Review)
        {
            return true;
        }
        return false;
    }

    public async ValueTask Handle(TestCaseSavingEvent notification, CancellationToken cancellationToken)
    {
        if (!IsApplicableRequest(notification))
        {
            return;
        }
        var test = notification.New;
        if (string.IsNullOrEmpty(test.TenantId))
        {
            return;
        }

        if (test.ReviewAssignedTo?.Count > 0)
        {
            // Reviewers already assigned
            foreach (var reviewer in test.ReviewAssignedTo)
            {
                // Reset vote when test goes to review state
                reviewer.Vote = 0;
            }

            return;
        }

        var suite = await _testCaseRepository.GetTestSuiteByIdAsync(test.TenantId, test.TestSuiteId);
        if (suite?.DefaultReviewers?.Count > 0)
        {
            test.ReviewAssignedTo ??= new();
            foreach (var reviewer in suite.DefaultReviewers)
            {
                var entry = new AssignedReviewer { Role = reviewer.Role, UserName = reviewer.UserName, Vote = 0 };
                test.ReviewAssignedTo.Add(entry);
            }
        }
    }
}
