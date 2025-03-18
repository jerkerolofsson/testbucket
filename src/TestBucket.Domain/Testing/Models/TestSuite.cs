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
    /// Created by user name
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Modified by user name
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Timestamp when the entity was Modified
    /// </summary>
    public DateTimeOffset Modified { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Test case description
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
}
