using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Mapping;

namespace TestBucket.Domain.Export.Handlers;

/// <summary>
/// Exports the team
/// </summary>
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
