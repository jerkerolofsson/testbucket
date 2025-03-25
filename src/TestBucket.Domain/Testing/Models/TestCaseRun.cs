namespace TestBucket.Domain.Testing.Models;

/// <summary>
/// Result of one executed test case
/// </summary>
[Table("testcaseruns")]
[Index(nameof(Created))]
[Index(nameof(Name))]
public class TestCaseRun : TestEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// User the test run is assigned to
    /// </summary>
    public long? AssignedToUserId { get; set; }

    /// <summary>
    /// User the test run is assigned to
    /// </summary>
    public string? AssignedToUserName { get; set; }

    /// <summary>
    /// Test result
    /// </summary>
    public TestResult Result { get; set; } = TestResult.NoRun;

    /// <summary>
    /// Test state
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Message/Error
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Callstack
    /// </summary>
    public string? CallStack { get; set; }

    /// <summary>
    /// Total time to execute the test, in milliseconds
    /// </summary>
    public int Duration { get; set; }

    /// <summary>

    /// <summary>
    /// ID of test case
    /// </summary>
    public long TestCaseId { get; set; }

    /// <summary>
    /// ID of test run
    /// </summary>
    public long TestRunId { get; set; }

    // Navigation
    public virtual IEnumerable<TestCaseRunField>? TestCaseRunFields { get; set; }
    public TestRun? TestRun { get; set; }
    public TestCase? TestCase { get; set; }
}
