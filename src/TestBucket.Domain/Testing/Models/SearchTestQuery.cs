
namespace TestBucket.Domain.Testing.Models;
public class SearchTestQuery : SearchQuery
{
    public long? TestSuiteId { get; set; }
    public long? FolderId { get; set; }
}
