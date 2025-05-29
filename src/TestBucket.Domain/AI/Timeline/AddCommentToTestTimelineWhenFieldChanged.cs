using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Comments;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.AI.Timeline;
public class AddCommentToTestTimelineWhenFieldChanged : INotificationHandler<TestCaseFieldChangedNotification>
{
    private readonly ICommentsManager _comments;

    public AddCommentToTestTimelineWhenFieldChanged(ICommentsManager comments)
    {
        _comments = comments;
    }

    private async Task OnComponentRemoved(TestCaseFieldChangedNotification notification, TestCaseField field)
    {
        string icon = TimelineAiIconHelper.GetTimelineIcon(notification.Principal, TbIcons.BoldDuoTone.UserCircle);
        var comment = new Comment
        {
            TestCaseId = field.TestCaseId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "remove-component",
            LoggedActionArgument = notification.OldField?.StringValue,
            LoggedActionIcon = icon
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }

    private async Task OnComponentAdded(TestCaseFieldChangedNotification notification, TestCaseField field)
    {
        string icon = TimelineAiIconHelper.GetTimelineIcon(notification.Principal, TbIcons.BoldDuoTone.UserCircle);
        var comment = new Comment
        {
            TestCaseId = field.TestCaseId,
            TenantId = field.TenantId,
            TestProjectId = field.FieldDefinition!.TestProjectId,
            LoggedAction = "add-component",
            LoggedActionArgument = field.StringValue,
            LoggedActionIcon = icon
        };
        await _comments.AddCommentAsync(notification.Principal, comment);
    }
    public async ValueTask Handle(TestCaseFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Field.FieldDefinition is not null)
        {
            string icon = TimelineAiIconHelper.GetTimelineIcon(notification.Principal, TbIcons.BoldDuoTone.UserCircle);

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
        }
    }
}
