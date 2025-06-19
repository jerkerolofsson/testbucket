namespace TestBucket.Domain.Testing.Models;

/// <summary>
/// This is a folder within a test suite
/// A test suite folder can have
/// - Child folders
/// - Test cases
/// </summary>
[Table("testsuite__folders")]
[Index(nameof(TenantId), nameof(Created))]
public class TestSuiteFolder : TestEntity
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
    /// Path of the folder, separated by /
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// IDs for the path, for navigation
    /// </summary>
    public long[]? PathIds { get; set; }

    /// <summary>
    /// Flag to indicate that the folder is a feature/function
    /// This is used by LLMs to provide relevant requirements to the suite
    /// </summary>
    public bool IsFeature { get; set; }

    /// <summary>
    /// Feature description
    /// </summary>
    public string? FeatureDescription { get; set; }

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

    /// <summary>
    /// Foreign key to parent folder. 
    /// If null it is a root folder
    /// </summary>
    public long? ParentId { get; set; }


    // Navigation

    public TestSuiteFolder? Parent { get; set; }
    public IEnumerable<TestCase>? TestCases { get; set; }
    public TestSuite? TestSuite { get; set; }
}
