using Mediator;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects.Events;

namespace TestBucket.Domain.Fields.SystemFields;

internal class EnsureSystemFieldsExistsWhenProjectIsCreated : 
    INotificationHandler<ProjectCreated>,
    INotificationHandler<ProjectUpdated>
{
    private IFieldDefinitionManager _fieldDefinitionManager;

    public EnsureSystemFieldsExistsWhenProjectIsCreated(IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask Handle(ProjectCreated notification, CancellationToken cancellationToken)
    {
         await CreateMissingFieldDefinitionsAsync(notification.Project);
    }
    public async ValueTask Handle(ProjectUpdated notification, CancellationToken cancellationToken)
    {
        await CreateMissingFieldDefinitionsAsync(notification.Project);
    }

    private async Task CreateMissingFieldDefinitionsAsync(TestProject project)
    {
        var principal = Impersonation.Impersonate(project.TenantId);

        foreach (var definition in SystemFieldDefinitions.Fixed)
        {
            var result = await _fieldDefinitionManager.SearchAsync(principal, new SearchFieldQuery 
            { 
                ProjectId = project.Id,
                Name = definition.Name, Offset = 0, Count= 1 });
            if(result.Count == 0)
            {
                definition.TestProjectId = project.Id;
                await _fieldDefinitionManager.AddAsync(principal, definition);
            }
        }
    }
}
