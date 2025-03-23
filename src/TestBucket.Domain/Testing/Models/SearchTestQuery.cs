
namespace TestBucket.Domain.Testing.Models;
public class SearchTestQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? TestSuiteId { get; set; }
    public long? FolderId { get; set; }

    /// <summary>
    /// Compare the folder. If false, FolderId is ignored
    /// </summary>
    public bool CompareFolder { get; set; } = true;

    /// <summary>
    /// If comparing folder, returns all descendants, not only the direct children
    /// </summary>
    public bool Recurse { get; set; } = false;

    /// <summary>
    /// Test category field
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Test priority field
    /// </summary>
    public string? Priority { get; set; }
}
