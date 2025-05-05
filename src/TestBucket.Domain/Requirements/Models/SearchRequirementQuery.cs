
namespace TestBucket.Domain.Requirements.Models;
public class SearchRequirementQuery : SearchQuery
{
    /// <summary>
    /// Compares folder id
    /// </summary>
    public bool CompareFolder { get; set; } = true;

    /// <summary>
    /// Parent folder
    /// </summary>
    public long? FolderId { get; set; }

    /// <summary>
    /// Specification
    /// </summary>
    public long? RequirementSpecificationId { get; set; }

    /// <summary>
    /// Type of requirement
    /// </summary>
    public string? RequirementType { get; set; }

    /// <summary>
    /// State of requirement
    /// </summary>
    public string? RequirementState { get; set; }
}
