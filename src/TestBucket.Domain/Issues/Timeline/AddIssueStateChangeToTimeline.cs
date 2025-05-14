using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;
using TestBucket.Domain.Comments;

using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Issues.Events;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Issues.Timeline;
internal class AddIssueStateChangeToTimeline : INotificationHandler<IssueStateChangedNotification>
{
    private readonly ICommentsManager _comments;

    public AddIssueStateChangeToTimeline(ICommentsManager comments)
    {
        _comments = comments;
    }
    public async ValueTask Handle(IssueStateChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Issue.State is not null)
        {
            var comment = new Comment
            {
                LocalIssueId = notification.Issue.Id,
                TenantId = notification.Issue.TenantId,
                TestProjectId = notification.Issue.TestProjectId,
                LoggedAction = "statechange",
                LoggedActionArgument = notification.Issue.State
            };
            await _comments.AddCommentAsync(notification.Principal, comment);
        }
    }
}
