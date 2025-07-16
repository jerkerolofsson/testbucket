using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;

namespace TestBucket.Domain.Requirements.Models;

[Table("requirements")]
[Index(nameof(Created))]
[Index(nameof(Name))]
[Index(nameof(ExternalId))]
[Index(nameof(TenantId), nameof(Slug))]
public class Requirement : RequirementEntity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 1 for first item. This is unique only per project
    /// </summary>
    public int? SequenceNumber { get; set; }

    /// <summary>
    /// External id
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// External Provider
    /// </summary>
    public string? ExternalProvider { get; set; }

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
    /// Requirement state
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Known state of the requiremnt
    /// </summary>
    public MappedRequirementState? MappedState { get; set; }

    /// <summary>
    /// Optional root requirement if this is a downstream requirement
    /// This is useful to quickly identify all child requirements without traversing, for example to understand the coverage of a feature
    /// </summary>
    public long? RootRequirementId { get; set; }

    /// <summary>
    /// Optional parent requirement if this is a downstream requirement
    /// </summary>
    public long? ParentRequirementId { get; set; }

    /// <summary>
    /// Read-only requirements are managed outside
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Type of requirement
    /// </summary>
    public string? RequirementType { get; set; }

    /// <summary>
    /// Assigned to user
    /// </summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Known type of the requiremnt
    /// </summary>
    public MappedRequirementType? MappedType { get; set; }

    /// <summary>
    /// ID of test suite
    /// </summary>
    public long RequirementSpecificationId { get; set; }

    /// <summary>
    /// ID of test suite folder
    /// </summary>
    public long? RequirementSpecificationFolderId { get; set; }

    /// <summary>
    /// Work progress
    /// </summary>
    public double? Progress { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// End/due date
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Text embedding for semantic search and classification.
    /// Consists of the Name and descriptions
    /// </summary>
    [Column(TypeName = "vector(384)")]
    public Pgvector.Vector? Embedding { get; set; }


    // Navigation
    public RequirementSpecification? RequirementSpecification { get; set; }
    public RequirementSpecificationFolder? RequirementSpecificationFolder { get; set; }
    public virtual IEnumerable<RequirementField>? RequirementFields { get; set; }
    public virtual List<RequirementTestLink>? TestLinks { get; set; }
    public virtual List<Comment>? Comments { get; set; }
}
