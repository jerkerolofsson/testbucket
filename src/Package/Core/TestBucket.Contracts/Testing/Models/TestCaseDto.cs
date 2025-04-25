namespace TestBucket.Contracts.Testing.Models;

public class TestCaseDto
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
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Test case description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Slug for the test case
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Identifier in an external system
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Slug for the team
    /// </summary>
    public string? Team { get; set; }

    /// <summary>
    /// Slug for the project
    /// </summary>
    public string? Project { get; set; }

}
