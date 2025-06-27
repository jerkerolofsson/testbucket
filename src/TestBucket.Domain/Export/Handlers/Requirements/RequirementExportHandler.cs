using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Mapping;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications.Requirements;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Export.Handlers.Requirements;

/// <summary>
/// Exports requirements and collections
/// </summary>
internal class RequirementExportHandler : INotificationHandler<ExportNotification>
{
    private readonly IRequirementRepository _requirementRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly IProjectRepository _projectRepository;

    public RequirementExportHandler(IFieldRepository fieldRepository, IRequirementRepository requirementRepository, IProjectRepository projectRepository)
    {
        _fieldRepository = fieldRepository;
        _requirementRepository = requirementRepository;
        _projectRepository = projectRepository;
    }

    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        if (!notification.Principal.HasPermission(PermissionEntityType.Requirement, PermissionLevel.Read))
        {
            return;
        }

        string tenantId = notification.TenantId;
        int offset = 0;
        int count = 20;
        var filters = new List<FilterSpecification<RequirementSpecification>>
        {
            new FilterByTenant<RequirementSpecification>(tenantId)
        };
        var response = await _requirementRepository.SearchRequirementSpecificationsAsync(filters, offset, count);
        while (response.Items.Length > 0)
        {
            foreach (var item in response.Items)
            {
                if (!notification.Options.Filter(item))
                {
                    continue;
                }
                var dto = item.ToDto();

                // Need to add project reference
                if (item.TestProjectId is not null)
                {
                    item.TestProject = item.TestProject ?? await _projectRepository.GetProjectByIdAsync(tenantId, item.TestProjectId.Value);
                    dto.ProjectSlug = item.TestProject?.Slug;
                }

                await notification.Sink.WriteJsonEntityAsync("requirements", "requirement-specification", item.Id.ToString(), dto, cancellationToken);

                await WriteRequirementsAsync(notification, tenantId, item, notification.Sink, cancellationToken);
            }

            offset += count;
            response = await _requirementRepository.SearchRequirementSpecificationsAsync(filters, offset, count);
        }
    }

    private async Task WriteRequirementsAsync(ExportNotification notification, string tenantId, RequirementSpecification specification, IDataExporterSink sink, CancellationToken cancellationToken)
    {
        int offset = 0;
        int count = 20;
        var filters = new List<FilterSpecification<Requirement>>
        {
            new FilterByTenant<Requirement>(tenantId),
            new FilterRequirementBySpecification(specification.Id)
        };
        var response = await _requirementRepository.SearchRequirementsAsync(filters, offset, count);
        while (response.Items.Length > 0)
        {
            foreach (var item in response.Items)
            {
                if (!notification.Options.Filter(item))
                {
                    continue;
                }
                var dto = item.ToDto();
                dto.SpecificationSlug = specification.Slug;
                dto.ProjectSlug = specification.TestProject?.Slug;
                if(item.ParentRequirementId is not null)
                {
                    var parentRequirement = await _requirementRepository.GetRequirementByIdAsync(tenantId, item.ParentRequirementId.Value);
                    dto.ParentRequirementSlug = parentRequirement?.Slug; 
                }

                var fields = await _fieldRepository.GetRequirementFieldsAsync(tenantId, item.Id);
                dto.Traits = fields.ToTraits();

                await sink.WriteJsonEntityAsync("requirements", "requirement", item.Id.ToString(), dto, cancellationToken);
            }

            offset += count;
            response = await _requirementRepository.SearchRequirementsAsync(filters, offset, count);
        }
    }
}
