using Mediator;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Issues.Events;

namespace TestBucket.Domain.Issues.States;
internal class UpdateClosedTimestampWhenIssueStateIsChanging : INotificationHandler<IssueStateChangingNotification>
{
    private readonly TimeProvider _timeProvider;

    public UpdateClosedTimestampWhenIssueStateIsChanging(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public ValueTask Handle(IssueStateChangingNotification notification, CancellationToken cancellationToken)
    {
        var updatedIssue = notification.Issue;
        if (updatedIssue.MappedState is MappedIssueState.Closed or MappedIssueState.Canceled)
        {
            updatedIssue.Closed = _timeProvider.GetUtcNow();
        }
        return ValueTask.CompletedTask;
    }
}
