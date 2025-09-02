
using TestBucket.Contracts.Comments;
using TestBucket.Contracts.Fields;
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
    /// Name for the test suite
    /// </summary>
    public string? TestSuiteName { get; set; }

    /// <summary>
    /// Type of test case
    /// </summary>
    public TestExecutionType ExecutionType { get; set; }

    /// <summary>
    /// Script type, for manual tests.
    /// </summary>
    public ScriptType ScriptType { get; set; }

    /// <summary>
    /// Pre-conditions
    /// </summary>
    public string? Preconditions { get; set; }

    /// <summary>
    /// Post-conditions
    /// </summary>
    public string? Postconditions { get; set; }
    /// <summary>
    /// Folder path
    /// </summary>
    public string? Path { get; set; }
    public string? ExternalDisplayId { get; set; }

    public List<TestStepDto> Steps { get; set; } = [];
    public List<CommentDto>? Comments { get; set; }
    public string? RunnerLanguage { get; set; }
}
