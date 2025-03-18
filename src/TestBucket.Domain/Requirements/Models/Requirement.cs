using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Requirements.Models;

[Table("requirements")]
[Index(nameof(Created))]
[Index(nameof(Name))]
public class Requirement
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// External id
    /// </summary>
    public string? ExternalId { get; set; }

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
    /// Name of the requirement
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// User friendly ID of the requirement
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Requirement description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Folder path for the requirement, separated with /
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// IDs for the path, for navigation
    /// </summary>
    public long[]? PathIds { get; set; }

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
    public long RequirementSpecificationId { get; set; }

    /// <summary>
    /// ID of test suite folder
    /// </summary>
    public long? RequirementSpecificationFolderId { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public TestProject? TestProject { get; set; }
    public RequirementSpecification? RequirementSpecification { get; set; }
    public RequirementSpecificationFolder? RequirementSpecificationFolder { get; set; }
    public virtual IEnumerable<TestCaseField>? TestCaseFields { get; set; }
}
