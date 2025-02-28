namespace TestBucket.Domain.Testing.Models;

[Table("testsuite__folders")]
[Index(nameof(TenantId), nameof(Created))]
public class TestSuiteFolder
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
    /// ID of project
    /// </summary>
    public long? TestProjectId { get; set; }

    /// <summary>
    /// ID of test suite
    /// </summary>
    public long TestSuiteId { get; set; }

    // Customization

    /// <summary>
    /// SVG icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// HTML color
    /// </summary>
    public string? Color { get; set; }
    // Navigation

    public long? ParentId { get; set; }
    public TestSuiteFolder? Parent { get; set; }
    public IEnumerable<TestCase>? TestCases { get; set; }
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
    public TestSuite? TestSuite { get; set; }
}
