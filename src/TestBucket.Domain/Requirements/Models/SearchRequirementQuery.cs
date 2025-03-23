
namespace TestBucket.Domain.Requirements.Models;
public class SearchRequirementQuery : SearchQuery
{
    public bool CompareFolder { get; set; } = true;
    public long? FolderId { get; set; }
    public long? RequirementSpecificationId { get; set; }
}
