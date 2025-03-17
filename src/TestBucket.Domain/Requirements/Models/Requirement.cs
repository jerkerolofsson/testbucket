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
    /// Timestamp when the test case was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Name of the test case
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// User friendly ID of the test case
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Test case description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Folder path for the test case, separated with /
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
