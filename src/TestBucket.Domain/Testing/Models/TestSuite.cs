using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Testing.Models;

[Table("testsuites")]
[Index(nameof(TenantId), nameof(Created))]
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
    /// Default branch/tag/commit when running CI/CD
    /// </summary>
    public string? DefaultCiCdRef { get; set; }

    /// <summary>
    /// Which integration to use
    /// </summary>
    public string? CiCdSystem { get; set; }
}
