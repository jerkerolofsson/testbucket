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

    public async ValueTask Handle(TestCaseSavingEvent notification, CancellationToken cancellationToken)
    {
        if (notification.New.TestProjectId is null ||
            notification.New.TenantId is null)
        {
            return;
        }
        if(notification.Old is null)
        {
            return;
        }

        var principal = notification.Principal;

        if (notification.Old.Description == notification.New.Description)
        {
            return;
        }
        var testCaseStates = await _stateService.GetTestCaseStatesAsync(principal, notification.New.TestProjectId.Value);
        var completedState = testCaseStates.FirstOrDefault(x => x.MappedState == Contracts.Testing.States.MappedTestState.Completed);
        var ongoingState = testCaseStates.FirstOrDefault(x => x.MappedState == Contracts.Testing.States.MappedTestState.Ongoing);
        if (completedState is not null && ongoingState is not null)
        {
            if (notification.New.MappedState == completedState.MappedState)
            {
                var settings = await _settingsProvider.GetDomainSettingsAsync<EditorSettings>(notification.New.TenantId, notification.New.TestProjectId.Value);
                if (settings?.ChangeStateToOngoingWhenEditingTests == true)
                {
                    notification.New.MappedState = ongoingState.MappedState;
                    notification.New.State = ongoingState.Name;
                }
            }
        }
    }
}
