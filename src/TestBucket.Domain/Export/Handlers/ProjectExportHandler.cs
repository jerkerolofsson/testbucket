using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Export.Handlers;

/// <summary>
/// Exports the project
/// </summary>
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
        if(!notification.Principal.HasPermission(PermissionEntityType.Project, PermissionLevel.Read))
        {
            return;
        }

        int offset = 0;
        int count = 20;
        var response = await _projectRepository.SearchAsync(notification.TenantId, new SearchQuery { Offset = offset, Count = count });
        while(response.Items.Length > 0)
        {
            foreach (var project in response.Items)
            {
                if (!notification.Options.Filter(project))
                {
                    continue;
                }

                // Get project that includes external systems
                var fullProject = await _projectRepository.GetProjectByIdAsync(notification.TenantId, project.Id);
                if(fullProject is null) continue;

                var dto = fullProject.ToDto(notification.Options.IncludeSensitiveDetails);

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
