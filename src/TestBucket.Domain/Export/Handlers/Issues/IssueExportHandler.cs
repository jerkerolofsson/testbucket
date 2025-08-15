using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Mapping;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Export.Handlers.Issues;

/// <summary>
/// Exports local issues
/// </summary>
public class IssueExportHandler : INotificationHandler<ExportNotification>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IFieldRepository _fieldRepository;

    public IssueExportHandler(IIssueRepository issueRepository, IFieldRepository fieldRepository)
    {
        _issueRepository = issueRepository;
        _fieldRepository = fieldRepository;
    }

    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        if(!notification.Principal.HasPermission(PermissionEntityType.Issue, PermissionLevel.Read))
        {
            return;
        }
        var tenantId = notification.TenantId;

        int offset = 0;
        int count = 20;
        var filters = new List<FilterSpecification<LocalIssue>>() { new FilterByTenant<LocalIssue>(tenantId) };
        var response = await _issueRepository.SearchAsync(filters, offset ,count);
        while(response.Items.Length > 0)
        {
            foreach (var issue in response.Items)
            {
                if (!notification.Options.Filter(issue))
                {
                    continue;
                }

                var dto = issue.ToDto();

                var fields = await _fieldRepository.GetIssueFieldsAsync(tenantId, issue.Id);
                dto.Traits = fields.ToTraits();
                dto.ProjectSlug = issue.TestProject?.Slug;

                await notification.Sink.WriteJsonEntityAsync("issues", "issue", issue.Id.ToString(), dto, cancellationToken);
            }

            offset += count;
            response = await _issueRepository.SearchAsync(filters, offset, count);
        }
    }
}
