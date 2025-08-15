using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Milestones.Mapping;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Export.Handlers.Issues;

/// <summary>
/// Exports local issues
/// </summary>
public class MilestoneExportHandler : INotificationHandler<ExportNotification>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IFieldRepository _fieldRepository;

    public MilestoneExportHandler(IMilestoneRepository issueRepository, IFieldRepository fieldRepository)
    {
        _milestoneRepository = issueRepository;
        _fieldRepository = fieldRepository;
    }

    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        if(!notification.Principal.HasPermission(PermissionEntityType.Issue, PermissionLevel.Read))
        {
            return;
        }
        var tenantId = notification.TenantId;

        var filters = new List<FilterSpecification<Milestone>>() { new FilterByTenant<Milestone>(tenantId) };
        var milestones = await _milestoneRepository.GetMilestonesAsync(filters);
        foreach (var milestone in milestones)
        {
            if (!notification.Options.Filter(milestone))
            {
                continue;
            }

            var dto = milestone.ToDto();
            dto.ProjectSlug = milestone.TestProject?.Slug;
            await notification.Sink.WriteJsonEntityAsync("milestones", "milestone", milestone.Id.ToString(), dto, cancellationToken);
        }
    }
}
