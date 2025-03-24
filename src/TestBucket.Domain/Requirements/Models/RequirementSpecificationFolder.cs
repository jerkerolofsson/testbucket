namespace TestBucket.Domain.Requirements.Models;

[Table("spec__folders")]
[Index(nameof(TenantId), nameof(Created))]
public class RequirementSpecificationFolder : ProjectEntity
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
    /// Description of section
    /// </summary>
    public string? Description { get; set; }


    /// <summary>
    /// ID of requirement specification
    /// </summary>
    public long RequirementSpecificationId { get; set; }

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
    public RequirementSpecificationFolder? Parent { get; set; }
    public IEnumerable<Requirement>? Requirements { get; set; }
    public RequirementSpecification? RequirementSpecification { get; set; }
}
