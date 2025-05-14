using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Issues.Events;

namespace TestBucket.Domain.Issues.Timeline;
internal class AddAssigneeChangeToTimeline : INotificationHandler<IssueAssignmentChanged>
{
    private readonly ICommentsManager _comments;

    public AddAssigneeChangeToTimeline(ICommentsManager comments)
    {
        _comments = comments;
    }
    public async ValueTask Handle(IssueAssignmentChanged notification, CancellationToken cancellationToken)
    {
        if (notification.Issue.AssignedTo is not null)
        {
            var comment = new Comment
            {
                LocalIssueId = notification.Issue.Id,
                TenantId = notification.Issue.TenantId,
                TestProjectId = notification.Issue.TestProjectId,
                LoggedAction = "assignedto",
                LoggedActionArgument = notification.Issue.AssignedTo,
                LoggedActionIcon = TbIcons.BoldDuoTone.UserCircle,
            };
            await _comments.AddCommentAsync(notification.Principal, comment);
        }
    }
}
