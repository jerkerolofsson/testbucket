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

namespace TestBucket.Domain.Export.Handlers;
public class ProjectExportHandler : INotificationHandler<ExportNotification>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFieldRepository _fieldRepository;

    public ProjectExportHandler(IProjectRepository projectRepository, IFieldRepository fieldRepository)
    {
        _projectRepository = projectRepository;
        _fieldRepository = fieldRepository;
    }

    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        int offset = 0;
        int count = 20;
        var response = await _projectRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        while(response.Items.Length > 0)
        {
            foreach (var project in response.Items)
            {
                // Get project that includes external systems
                var fullProject = await _projectRepository.GetProjectByIdAsync(notification.TenantId, project.Id);
                if(fullProject is null) continue;

                var dto = fullProject.ToDto();
                dto.Team = fullProject.Team?.Slug;

                await notification.Sink.WriteJsonEntityAsync("projects", "project", project.Id.ToString(), dto, cancellationToken);

                var fields = await _fieldRepository.SearchAsync([
                    new FilterByTenant<FieldDefinition>(notification.TenantId),
                    new FilterByProject<FieldDefinition>(project.Id)]);
                foreach (var fieldDefinition in fields)
                {
                    await notification.Sink.WriteJsonEntityAsync("projects", "field", fieldDefinition.Id.ToString(), fieldDefinition.ToDto(project.Slug), cancellationToken);
                }
            }

            offset += count;
            response = await _projectRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        }
    }
}
