using Mediator;

using TestBucket.Domain.Editor.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Events;

namespace TestBucket.Domain.Testing.TestCases.Features.ChangeStateToOngoingWhenEditingTests;
public class ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted : INotificationHandler<TestCaseSavingEvent>
{
    private readonly IStateService _stateService;
    private readonly ISettingsProvider _settingsProvider;

    public ChangeStateToOngoingWhenTestIsSavedIfStateIsCompleted(IStateService stateService, ISettingsProvider settingsProvider)
    {
        _stateService = stateService;
        _settingsProvider = settingsProvider;
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

        // If the description hasn't changed, don't do anything
        if (notification.Old.Description == notification.New.Description)
        {
            return false;
        }
        return true;
    }

    public async ValueTask Handle(TestCaseSavingEvent notification, CancellationToken cancellationToken)
    {
        if (!IsApplicableRequest(notification))
        {
            return;
        }

        // Read settings
        var settings = await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(notification.New.TenantId!, notification.New.TestProjectId!.Value);
        var enabled = settings?.ChangeStateToOngoingWhenEditingTests == true;

        if(!enabled)
        {
            return;
        }

        var principal = notification.Principal;

        // Read the states to determine the applicable state to trigger and the target state
        var testCaseStates = await _stateService.GetTestCaseStatesAsync(principal, notification.New.TestProjectId!.Value);
        var completedState = testCaseStates.FirstOrDefault(x => x.MappedState == Contracts.Testing.States.MappedTestState.Completed);
        var ongoingState = testCaseStates.FirstOrDefault(x => x.MappedState == Contracts.Testing.States.MappedTestState.Ongoing);

        if (completedState is not null && ongoingState is not null && notification.New.MappedState == completedState.MappedState)
        {
            notification.New.MappedState = ongoingState.MappedState;
            notification.New.State = ongoingState.Name;
        }
    }
}
