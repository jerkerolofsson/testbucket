namespace TestBucket.Domain.Testing.Models;

/// <summary>
/// Result of one executed test case
/// </summary>
[Table("testcaseruns")]
[Index(nameof(Created))]
[Index(nameof(Name))]
public class TestCaseRun
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
    /// ID of tenant
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// ID of project
    /// </summary>
    public long TestProjectId { get; set; }

    /// <summary>
    /// ID of test case
    /// </summary>
    public long TestCaseId { get; set; }

    /// <summary>
    /// ID of test run
    /// </summary>
    public long TestRunId { get; set; }

    // Navigation
    public TestRun? TestRun { get; set; }
    public TestCase? TestCase { get; set; }
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
}
