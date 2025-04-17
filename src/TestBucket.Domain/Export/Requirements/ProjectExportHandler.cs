using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;

namespace TestBucket.Domain.Export.Requirements;
public class ProjectExportHandler : INotificationHandler<ExportNotification>
{
    private readonly IProjectRepository _projectRepository;

    public ProjectExportHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        int offset = 0;
        int count = 20;
        var response = await _projectRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        while(response.TotalCount > 0)
        {
            foreach (var project in response.Items)
            {
                var dto = project.ToDto();

                using var stream = new MemoryStream();
                var options = new JsonSerializerOptions { WriteIndented = true };
                await JsonSerializer.SerializeAsync(stream, dto, options, cancellationToken);
                await notification.Sink.WriteEntityAsync("projects", "project", project.Id.ToString(), stream, cancellationToken);
            }

            offset += count;
            response = await _projectRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        }
    }
}
