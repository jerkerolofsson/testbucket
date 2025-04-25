using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects.Events;

namespace TestBucket.Domain.States.Caching;
internal class UpdateCacheWhenProjectIsSaved : INotificationHandler<ProjectUpdated>, INotificationHandler<ProjectCreated>
{
    private readonly ProjectStateCache _cache;

    public UpdateCacheWhenProjectIsSaved(ProjectStateCache cache)
    {
        _cache = cache;
    }

    public ValueTask Handle(ProjectUpdated notification, CancellationToken cancellationToken)
    {
        _cache.Update(notification.Project);
        return ValueTask.CompletedTask;
    }

    public ValueTask Handle(ProjectCreated notification, CancellationToken cancellationToken)
    {
        _cache.Update(notification.Project);
        return ValueTask.CompletedTask;
    }
}
