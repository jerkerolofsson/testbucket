using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Comments;
using TestBucket.Domain.Fields.Events;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Issues.Timeline;
internal class AddCommitEventWhenFieldUpdated : INotificationHandler<IssueFieldChangedNotification>
{
    private readonly ICommentsManager _comments;

    public AddCommitEventWhenFieldUpdated(ICommentsManager comments)
    {
        _comments = comments;
    }
    public async ValueTask Handle(IssueFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        if(notification.Field.FieldDefinition is not null)
        {
            if (notification.Field.FieldDefinition.TraitType == TraitType.Commit)
            {
                var field = notification.Field;
                if (!string.IsNullOrEmpty(field.StringValue))
                {
                    var comment = new Comment
                    {
                        LocalIssueId = field.LocalIssueId,
                        TenantId = field.TenantId,
                        TestProjectId = field.FieldDefinition.TestProjectId,
                        LoggedAction = "commit",
                        LoggedActionArgument = field.StringValue,
                        LoggedActionIcon = TbIcons.Git.Commit
                    };
                    await _comments.AddCommentAsync(notification.Principal, comment);
                }
            }
        }
    }
}
