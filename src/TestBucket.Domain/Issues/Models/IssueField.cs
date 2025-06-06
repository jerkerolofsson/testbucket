

namespace TestBucket.Domain.Issues.Models;

[Table("issue_fields")]
[Index(nameof(TenantId), nameof(LocalIssueId))]
public class IssueField : FieldValue
{

    // Navigation

    public required long LocalIssueId { get; set; }
    public LocalIssue? LocalIssue { get; set; }
}
