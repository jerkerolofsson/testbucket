using TestBucket.Contracts.Comments;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Requirements;
public class RequirementDto : RequirementEntityDto
{
    /// <summary>
    /// Slug for the specification
    /// </summary>
    public string? SpecificationSlug { get; set; }

    public string? State { get; set; }
    public string? Path { get; set; }
    public MappedRequirementState? MappedState { get; set; }
    public string? RequirementType { get; set; }
    public MappedRequirementType? MappedType { get; set; }

    /// <summary>
    /// Slug for the parent directory
    /// </summary>
    public string? ParentRequirementSlug { get; set; }

    /// <summary>
    /// Contains traits/custom properties
    /// </summary>
    public TestTraitCollection? Traits { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public double? Progress { get; set; }

    public List<CommentDto>? Comments { get; set; }
}
