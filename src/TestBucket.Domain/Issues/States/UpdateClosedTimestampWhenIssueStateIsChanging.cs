using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Issues.Events;

namespace TestBucket.Domain.Issues.States;
internal class UpdateClosedTimestampWhenIssueStateIsChanging : INotificationHandler<IssueStateChangingNotification>
{
    public ValueTask Handle(IssueStateChangingNotification notification, CancellationToken cancellationToken)
    {
        var updatedIssue = notification.Issue;
        if (updatedIssue.MappedState is MappedIssueState.Closed or MappedIssueState.Canceled)
        {
            updatedIssue.Closed = DateTimeOffset.UtcNow;
        }
        return ValueTask.CompletedTask;
    }
}
