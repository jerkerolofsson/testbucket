using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Issues.Models;

public class LinkedIssue : ProjectEntity
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
    /// Milestone
    /// </summary>
    public string? MilestoneName { get; set; }

    /// <summary>
    /// Type of issue
    /// </summary>
    public string? IssueType { get; set; }

    /// <summary>
    /// ID to display in UI
    /// </summary>
    public string? ExternalDisplayId { get; set; }

    // Navigation

    public long? LocalIssueId { get; set; }
    public LocalIssue? LocalIssue { get; set; }

    public long? TestCaseRunId { get; set; }
    public TestCaseRun? TestCaseRun { get; set; }
}
