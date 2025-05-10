using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects.Events;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Fields.SystemFields;

internal class EnsureSystemFieldsExistsWhenProjectIsCreated : 
    INotificationHandler<ProjectCreated>,
    INotificationHandler<ProjectUpdated>
{
    private readonly ILogger<EnsureSystemFieldsExistsWhenProjectIsCreated> _logger;
    private readonly IFieldRepository _fieldRepo;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public EnsureSystemFieldsExistsWhenProjectIsCreated(
        ILogger<EnsureSystemFieldsExistsWhenProjectIsCreated> logger,
        IFieldRepository fieldRepo,
        IFieldDefinitionManager fieldDefinitionManager)
    {
        _logger = logger;
        _fieldRepo = fieldRepo;
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
        var tenantId = principal.GetTenantIdOrThrow();

        foreach (var definition in SystemFieldDefinitions.Fixed)
        {
            IReadOnlyList<FilterSpecification<FieldDefinition>> specifications = [
                new FilterByProject<FieldDefinition>(project.Id),
                new FilterFieldDefinitionByName(definition.Name),
                new FilterByTenant<FieldDefinition>(tenantId)
            ];
            var results = await _fieldRepo.SearchAsync(specifications);
            if(results.Count == 0)
            {
                _logger.LogInformation("Creating system field: {FieldDefinitionName} for project: {ProjectName}", definition.Name, project.Name);
                try
                {
                    await CreateSystemFieldDefinitionAsync(project, principal, definition);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error creating system field: {FieldDefinitionName} for project: {ProjectName}", definition.Name, project.Name);
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("System field already exists: {FiledDefinitionName} for project: {ProjectName}", definition.Name, project.Name);
            }
        }

        _fieldDefinitionManager.ClearTenantCache(tenantId);
    }

    private async Task CreateSystemFieldDefinitionAsync(TestProject project, ClaimsPrincipal principal, FieldDefinition definition)
    {
        var newField = new FieldDefinition
        {
            TenantId = project.TenantId,
            TeamId = project.TeamId,
            TestProjectId = project.Id,
            Name = definition.Name,
            DataSourceUri = definition.DataSourceUri,
            DataSourceType = definition.DataSourceType,
            Description = definition.Description,
            Inherit = definition.Inherit,
            UseClassifier = definition.UseClassifier,
            IsVisible = definition.IsVisible,
            TraitType = definition.TraitType,
            WriteOnly = definition.WriteOnly,
            ReadOnly = definition.ReadOnly,
            Trait = definition.Trait,
            Type = definition.Type,
            OptionIcons = definition.OptionIcons,
            Options = definition.Options,
            Icon = definition.Icon,
            IsDefinedBySystem = definition.IsDefinedBySystem,
            Target = definition.Target,
            ShowDescription = definition.ShowDescription,
            RequiredPermission = definition.RequiredPermission
        };
        await _fieldRepo.AddAsync(newField);
    }
}
