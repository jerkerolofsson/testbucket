using Mediator;

using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.AI.Runner.Integrations;
internal class WakeRunnerWhenTestIsAssigned : INotificationHandler<TestCaseRunAssignmentChangedNotification>
{
    private readonly AiRunnerJobQueue _queue;

    public WakeRunnerWhenTestIsAssigned(AiRunnerJobQueue queue)
    {
        _queue = queue;
    }

    public async ValueTask Handle(TestCaseRunAssignmentChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.TestCaseRun.AssignedToUserName == "ai-runner")
        {
            await _queue.EnqueueAsync(notification.TestCaseRun);
        }
    }
}
