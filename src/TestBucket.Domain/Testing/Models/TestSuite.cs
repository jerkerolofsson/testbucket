using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("testsuites")]
[Index(nameof(TenantId), nameof(Created))]
[Index(nameof(TenantId), nameof(Slug))]
public class TestSuite : TestEntity
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
    /// Slug for the test suite
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Test suite description
    /// </summary>
    public string? Description { get; set; }

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
    /// Variables for the test suite, typically for CI/CD
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? Variables { get; set; }

    /// <summary>
    /// Dependencies / resource requirements
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<TestCaseDependency>? Dependencies { get; set; }

    /// <summary>
    /// Default branch/tag/commit when running CI/CD
    /// </summary>
    public string? DefaultCiCdRef { get; set; }

    /// <summary>
    /// The workflow name (used by Github)
    /// </summary>
    public string? CiCdWorkflow { get; set; }

    /// <summary>
    /// If true, pipelines started from an outside source (e.g. regular CI pipeline) will be indexed and runs added
    /// </summary>
    public bool? AddPipelinesStartedFromOutside { get; set; }

    /// <summary>
    /// Which integration to use
    /// </summary>
    public string? CiCdSystem { get; set; }

    /// <summary>
    /// Which integration to use
    /// </summary>
    public long? ExternalSystemId { get; set; }

    // Navigation

    public virtual List<Comment>? Comments { get; set; }
}
