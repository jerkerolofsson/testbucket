using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects.Events;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics.Defaults;
public class AddDefaultHeuristicsWhenProjectIsAdded : INotificationHandler<ProjectCreated>
{
    private readonly IHeuristicsManager _manager;

    public AddDefaultHeuristicsWhenProjectIsAdded(IHeuristicsManager manager)
    {
        _manager = manager;
    }

    public async ValueTask Handle(ProjectCreated notification, CancellationToken cancellationToken)
    {
        foreach(var heuristic in DefaultHeuristics.Defaults)
        {
            var projectHeuristic = new Heuristic
            {
                Name = heuristic.Name,
                Description = heuristic.Description,
                TestProjectId = notification.Project.Id,
            };
            await _manager.AddAsync(notification.Principal, projectHeuristic);
        }
    }
}
