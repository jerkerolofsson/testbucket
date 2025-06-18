using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Metrics.Models;

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
    /// Slug
    /// </summary>
    public string? Slug { get; set; }

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
    /// Mapped state
    /// </summary>
    public MappedTestState? MappedState { get; set; }

    /// <summary>
    /// Charter for exploratory testing
    /// Copied from TestCase.Description when starting a new test
    /// </summary>
    public string? Charter { get; set; }

    /// <summary>
    /// Copied from test case
    /// </summary>
    public ScriptType ScriptType { get; set; }

    /// <summary>
    /// Message/Error/Session Log
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Callstack
    /// </summary>
    public string? CallStack { get; set; }

    /// <summary>
    /// stdout during test
    /// </summary>
    public string? SystemOut { get; set; }

    /// <summary>
    /// stderr during test
    /// </summary>
    public string? SystemErr { get; set; }

    /// <summary>
    /// Estimated duration to run the test, in milliseconds
    /// or
    /// Planned session duration for exploratory testing
    /// </summary>
    public int? Estimate { get; set; }

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
    public virtual List<LinkedIssue>? LinkedIssues { get; set; }
    public virtual List<Metric>? Metrics { get; set; }
    public virtual List<Comment>? Comments { get; set; }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        if(obj is TestCaseRun other)
        {
            return Id == other.Id;
        }
        return base.Equals(obj);
    }


}
