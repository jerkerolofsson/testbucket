
using Mediator;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRuns.Events;

/// <summary>
/// This event is sent before a TestCaseRun is saved.
/// </summary>
/// <param name="Principal"></param>
public record class TestCaseRunUpdatingNotification(ClaimsPrincipal Principal, TestCaseRun Old, TestCaseRun New) : INotification;

public class UpdateStatesWhenTestCaseRunAssignmentIsUpdated : INotificationHandler<TestCaseRunUpdatingNotification>
{
    private readonly IStateService _stateService;

    public UpdateStatesWhenTestCaseRunAssignmentIsUpdated(IStateService stateService)
    {
        _stateService = stateService;
    }

    public async ValueTask Handle(TestCaseRunUpdatingNotification notification, CancellationToken cancellationToken)
    {
        bool assignmentChanged = notification.Old.AssignedToUserName != notification.New.AssignedToUserName;
        bool wasUnassigned = assignmentChanged && string.IsNullOrEmpty(notification.New.AssignedToUserName);
        bool wasAssigned = assignmentChanged && !string.IsNullOrEmpty(notification.New.AssignedToUserName);

        if (wasUnassigned)
        {
            if (notification.New.MappedState == MappedTestState.Assigned)
            {
                await SetStateAsync(notification.Principal, notification.New, MappedTestState.NotStarted);
            }
        }
        else if (wasAssigned)
        {
            if (notification.New.MappedState == MappedTestState.NotStarted)
            {
                await SetStateAsync(notification.Principal, notification.New, MappedTestState.Assigned);
            }
        }
    }

    private async Task SetStateAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun, MappedTestState newState)
    {
        if (testCaseRun.TestProjectId is not null)
        {
            var states = await _stateService.GetTestCaseRunStatesAsync(principal, testCaseRun.TestProjectId!.Value);
            var state = states.Where(x => x.MappedState == newState).FirstOrDefault();
            if (state is not null)
            {
                testCaseRun.State = state.Name;
            }
            testCaseRun.MappedState = newState;
        }
    }
}