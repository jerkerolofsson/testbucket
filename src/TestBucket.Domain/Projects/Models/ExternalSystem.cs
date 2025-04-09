namespace TestBucket.Domain.Projects.Models;

/// <summary>
/// Represents an integration with an external system
/// </summary>
[Table("external_systems")]
public class ExternalSystem : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// Name of the external system
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Base URL in the external system for API access
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// ID for the project in the external system
    /// </summary>
    public string? ExternalProjectId { get; set; }

    /// <summary>
    /// Access token for the external system
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// API provides only read-only access
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// True if enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Enable reading releases from this system
    /// </summary>
    public bool EnableReleases { get; set; }

    /// <summary>
    /// Enable reading milestones from this system
    /// </summary>
    public bool EnableMilestones { get; set; }
}
