using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Fields.Events;

namespace TestBucket.Domain.Requirements.Events;
internal class CallObserversWhenRequirementFieldIsChanged : INotificationHandler<RequirementFieldChangedNotification>
{
    private readonly IRequirementManager _requirementManager;

    public CallObserversWhenRequirementFieldIsChanged(IRequirementManager requirementManager)
    {
        _requirementManager = requirementManager;
    }

    public async ValueTask Handle(RequirementFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        await _requirementManager.OnRequirementFieldChangedAsync(notification.Principal, notification.Field);
    }
}
