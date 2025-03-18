namespace TestBucket.Domain.Testing.Models;

/// <summary>
/// Represents a group of tests executed together
/// </summary>
[Table("stepruns")]
public class TestStepRun : TestEntity
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
    /// Result of the step
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// ID of run
    /// </summary>
    public long TestCaseRunId { get; set; }

    // Navigation
    public TestCaseRun? TestCaseRun { get; set; }
}
