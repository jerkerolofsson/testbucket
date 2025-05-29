using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.AI;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Issues.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Issues.Timeline;
internal class AddEventsWhenFieldUpdated : INotificationHandler<IssueFieldChangedNotification>
{
    private readonly ICommentsManager _comments;

    public AddEventsWhenFieldUpdated(ICommentsManager comments)
    {
        _comments = comments;
    }
    public async ValueTask Handle(IssueFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        if(notification.Field.FieldDefinition is not null)
        {
            if (notification.Field.FieldDefinition.TraitType == TraitType.Component)
            {
                var field = notification.Field;
                if (!string.IsNullOrEmpty(field.StringValue))
                {
                    // Change milestone
                    if (notification.OldField?.StringValue is not null)
                    {
                        await OnComponentRemoved(notification, field);
                        await OnComponentAdded(notification, field);
                    }
                    else
                    {
                        await OnComponentAdded(notification, field);
                    }
                }
                else if (notification.OldField?.StringValue is not null)
                {
                    await OnComponentRemoved(notification, field);
                }
            }

            if (notification.Field.FieldDefinition.TraitType == TraitType.Milestone)
            {
                var field = notification.Field;
                if (!string.IsNullOrEmpty(field.StringValue))
                {
                    // Change milestone
                    if (notification.OldField?.StringValue is not null)
                    {
                        await OnMilstoneRemoved(notification, field);
                        await OnMilestoneAdded(notification, field);
                    }
                    else
                    {
                        await OnMilestoneAdded(notification, field);
                    }
                }
                else if (notification.OldField?.StringValue is not null)
                {
                    await OnMilstoneRemoved(notification, field);
                }
            }

            if (notification.Field.FieldDefinition.TraitType == TraitType.Commit)
            {
                var field = notification.Field;
                if (!string.IsNullOrEmpty(field.StringValue))
                {
                    await OnCommitAdded(notification, field);
                }
            }
        }
    }

    private async Task OnCommitAdded(IssueFieldChangedNotification notification, Models.IssueField field)
    {
        var comment = new Comment
        {
            LocalIssueId = field.LocalIssueId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "commit",
            LoggedActionArgument = field.StringValue,
            LoggedActionIcon = TbIcons.Git.Commit
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }

    private async Task OnMilstoneRemoved(IssueFieldChangedNotification notification, Models.IssueField field)
    {
        var comment = new Comment
        {
            LocalIssueId = field.LocalIssueId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "remove-milestone",
            LoggedActionArgument = notification.OldField?.StringValue,
            LoggedActionIcon = TbIcons.BoldDuoTone.Flag
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }

    private async Task OnMilestoneAdded(IssueFieldChangedNotification notification, Models.IssueField field)
    {
        var comment = new Comment
        {
            LocalIssueId = field.LocalIssueId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "add-milestone",
            LoggedActionArgument = field.StringValue,
            LoggedActionIcon = TbIcons.BoldDuoTone.Flag
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }

    private async Task OnComponentRemoved(IssueFieldChangedNotification notification, IssueField field)
    {
        string icon = TimelineAiIconHelper.GetTimelineIcon(notification.Principal, TbIcons.BoldDuoTone.UserCircle);
        var comment = new Comment
        {
            LocalIssueId = field.LocalIssueId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "remove-component",
            LoggedActionArgument = notification.OldField?.StringValue,
            LoggedActionIcon = icon
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }

    private async Task OnComponentAdded(IssueFieldChangedNotification notification, IssueField field)
    {
        string icon = TimelineAiIconHelper.GetTimelineIcon(notification.Principal, TbIcons.BoldDuoTone.UserCircle);
        var comment = new Comment
        {
            LocalIssueId = field.LocalIssueId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "add-component",
            LoggedActionArgument = field.StringValue,
            LoggedActionIcon = icon
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }
}
