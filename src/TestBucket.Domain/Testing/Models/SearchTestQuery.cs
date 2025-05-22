

using OneOf.Types;

using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.Testing.Models;
public class SearchTestQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? TestSuiteId { get; set; }
    public long? FolderId { get; set; }

    /// <summary>
    /// If true, excludes automated tests
    /// </summary>
    public bool? ExcludeAutomated { get; set; }

    /// <summary>
    /// Compare the folder. If false, FolderId is ignored
    /// </summary>
    public bool? CompareFolder { get; set; }

    /// <summary>
    /// If comparing folder, returns all descendants, not only the direct children
    /// </summary>
    public bool Recurse { get; set; } = false;

    /// <summary>
    /// Filter on test execution type
    /// </summary>
    public TestExecutionType? TestExecutionType { get; set; }

    public override int GetHashCode() => this.ToSearchText().GetHashCode();

    public override bool Equals(object? obj)
    {
        if(obj is SearchTestQuery other)
        {
            var a = this.ToSearchText();
            var b = other.ToSearchText();
            return a.Equals(b);
        }
        return false;
    }
}
