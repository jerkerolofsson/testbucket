using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Testing.Models;

public class TestCaseDto
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Traits for the test suite
    /// </summary>
    public TestTraitCollection? Traits { get; set; }

    /// <summary>
    /// Timestamp when the test case was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string TestCaseName { get; set; }

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
    /// Slug for the team
    /// </summary>
    public string? TeamSlug { get; set; }

    /// <summary>
    /// Slug for the project
    /// </summary>
    public string? ProjectSlug { get; set; }

    /// <summary>
    /// Slug for the test suite
    /// </summary>
    public string? TestSuiteSlug { get; set; }

    /// <summary>
    /// Type of test case
    /// </summary>
    public TestExecutionType ExecutionType { get; set; }
}
