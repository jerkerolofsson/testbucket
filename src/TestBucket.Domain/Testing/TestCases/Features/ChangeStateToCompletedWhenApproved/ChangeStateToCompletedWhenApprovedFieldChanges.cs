using Mediator;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.States;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Testing.TestCases.Features.ChangeStateToCompletedWhenApproved;
internal class ChangeStateToCompletedWhenApprovedFieldChanges : INotificationHandler<TestCaseFieldChangedNotification>
{
    private readonly ITestCaseManager _testCaseManager;
    private readonly IStateService _stateService;

    public ChangeStateToCompletedWhenApprovedFieldChanges(ITestCaseManager testCaseManager, IStateService stateService)
    {
        _testCaseManager = testCaseManager;
        _stateService = stateService;
    }

    public async ValueTask Handle(TestCaseFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        var fieldDefinition = notification.Field.FieldDefinition;
        if (fieldDefinition is null)
        {
            return;
        }

        if(fieldDefinition.TraitType == TraitType.Approved && notification.Field.BooleanValue == true)
        {
            var testCase = await _testCaseManager.GetTestCaseByIdAsync(notification.Principal, notification.Field.TestCaseId);
            if(testCase?.TestProjectId is not null && testCase.MappedState != MappedTestState.Completed)
            {
                var states = await _stateService.GetTestCaseStatesAsync(notification.Principal, testCase.TestProjectId.Value);
                var completedState = states.FirstOrDefault(x => x.MappedState == MappedTestState.Completed);

                if(completedState is not null)
                {
                    testCase.MappedState = completedState.MappedState;
                    testCase.State = completedState.Name;
                    await _testCaseManager.SaveTestCaseAsync(notification.Principal, testCase);
                }
            }
        }
    }
}
