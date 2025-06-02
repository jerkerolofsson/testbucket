using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Projects;
using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Mapping;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Export.Handlers;
public class TeamExportHandler : INotificationHandler<ExportNotification>
{
    private readonly ITeamRepository _teamRepository;

    public TeamExportHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        if (!notification.Principal.HasPermission(PermissionEntityType.Team, PermissionLevel.Read))
        {
            return;
        }

        int offset = 0;
        int count = 20;
        var response = await _teamRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        while(response.Items.Length > 0)
        {
            foreach (var team in response.Items)
            {
                if (!notification.Options.Filter(team))
                {
                    continue;
                }

                await notification.Sink.WriteJsonEntityAsync("teams", "team", team.Id.ToString(), team.ToDto(), cancellationToken);
            }

            offset += count;
            response = await _teamRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        }
    }
}
