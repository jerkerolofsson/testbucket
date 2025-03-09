using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Testing.Models;

/// <summary>
/// Represents a group of tests executed together
/// </summary>
[Table("runs")]
[Index(nameof(TenantId), nameof(Created))]
public class TestRun
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Timestamp when the test case was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Created by
    /// </summary>
    public string? Creator { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Test case description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// stdout
    /// </summary>
    public string? SystemOut { get; set; }

    /// <summary>
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// ID of project
    /// </summary>
    public long? TestProjectId { get; set; }

    /// <summary>
    /// ID of team
    /// </summary>
    public long? TeamId { get; set; }

    /// <summary>
    /// External identifer, other system, or when importing
    /// </summary>
    public string? ExternalId { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
    public Team? Team { get; set; }
}    
