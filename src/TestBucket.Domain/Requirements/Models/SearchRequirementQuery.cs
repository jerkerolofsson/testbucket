
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
}
