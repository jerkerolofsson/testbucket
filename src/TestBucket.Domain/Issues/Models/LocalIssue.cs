namespace TestBucket.Domain.Issues.Models;

[Index(nameof(State),nameof(Created))]
[Index(nameof(ExternalSystemId), nameof(Created))]
public class LocalIssue : ProjectEntity
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id in external system
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Textual identifier for the external system, e.g. jira, github, gitlab
    /// 
    /// If null, assume the issue is local only
    /// </summary>
    public string? ExternalSystemName { get; set; }

    /// <summary>
    /// Identifier for the external system configuration
    /// </summary>
    public long? ExternalSystemId { get; set; }

    /// <summary>
    /// Title of issue
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of issue
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// State
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Author/Creator
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// External URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Type of issue
    /// </summary>
    public string? IssueType { get; set; }

    /// <summary>
    /// ID to display in UI
    /// </summary>
    public string? ExternalDisplayId { get; set; }

    // Navigation

    public virtual IEnumerable<IssueField>? IssueFields { get; set; }

}
