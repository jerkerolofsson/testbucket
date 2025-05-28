using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Fields.Events;

namespace TestBucket.Domain.Issues.Events;
internal class CallObserversWhenIssueFieldIsChanged : INotificationHandler<IssueFieldChangedNotification>
{
    private readonly IIssueManager _issueManager;

    public CallObserversWhenIssueFieldIsChanged(IIssueManager issueManager)
    {
        _issueManager = issueManager;
    }

    public async ValueTask Handle(IssueFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        await _issueManager.OnLocalIssueFieldChangedAsync(notification.Principal, notification.Field);
    }
}
