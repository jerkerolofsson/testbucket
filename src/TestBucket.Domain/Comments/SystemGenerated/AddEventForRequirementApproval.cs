using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Fields.Events;

namespace TestBucket.Domain.Comments.SystemGenerated;
internal class AddEventForRequirementApproval : INotificationHandler<RequirementFieldChangedNotification>
{
    private readonly ICommentsManager _comments;

    public AddEventForRequirementApproval(ICommentsManager comments)
    {
        _comments = comments;
    }

    public async ValueTask Handle(RequirementFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        var field = notification.Field;
        if (field.FieldDefinition is null)
        {
            return;
        }
        if(field.FieldDefinition.TraitType == Traits.Core.TraitType.Approved)
        {
            if(field.BooleanValue == true)
            {
                var comment = new Comment
                {
                    RequirementId = field.RequirementId,
                    TenantId = field.TenantId,
                    TestProjectId = field.FieldDefinition.TestProjectId,
                    LoggedAction = "approved",
                };
                await _comments.AddCommentAsync(notification.Principal, comment);
            }
        }
    }
}
