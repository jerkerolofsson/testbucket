
namespace TestBucket.Domain.Issues.Models;

[Table("issue_fields")]
public class IssueField : FieldValue
{

    // Navigation

    public required long LocalIssueId { get; set; }
    public LocalIssue? LocalIssue { get; set; }
}
