using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Comments;
using TestBucket.Domain.Fields.Events;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.AI.Timeline;
public class AddCommentToTestTimelineWhenFieldChanged : INotificationHandler<TestCaseFieldChangedNotification>
{
    private readonly ICommentsManager _comments;

    public AddCommentToTestTimelineWhenFieldChanged(ICommentsManager comments)
    {
        _comments = comments;
    }
    public async ValueTask Handle(TestCaseFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Field.FieldDefinition is not null)
        {
            string icon = TbIcons.BoldDuoTone.Components;
            if (notification.Principal.Identity?.Name is not null)
            {
                var model = LlmModels.GetModelByName(notification.Principal.Identity.Name);
                if (model?.Icon is not null)
                {
                    icon = model.Icon;
                }
            }


            if (notification.Field.FieldDefinition.TraitType == TraitType.Component)
            {
                var field = notification.Field;
                if (!string.IsNullOrEmpty(field.StringValue))
                {
                    var comment = new Comment
                    {
                        TestCaseId = field.TestCaseId,
                        TenantId = field.TenantId,
                        TestProjectId = field.FieldDefinition.TestProjectId,
                        LoggedAction = "component",
                        LoggedActionArgument = field.StringValue,
                        LoggedActionIcon = icon
                    };
                    await _comments.AddCommentAsync(notification.Principal, comment);
                }
                else
                {
                    var comment = new Comment
                    {
                        TestCaseId = field.TestCaseId,
                        TenantId = field.TenantId,
                        TestProjectId = field.FieldDefinition.TestProjectId,
                        LoggedAction = "component-removed",
                        LoggedActionArgument = field.StringValue,
                        LoggedActionIcon = icon
                    };
                    await _comments.AddCommentAsync(notification.Principal, comment);
                }
            }
        }
    }
}
