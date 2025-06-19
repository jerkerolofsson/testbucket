namespace TestBucket.Domain.Testing.TestSuites.Search;

public class SearchTestSuiteQuery : SearchQuery
{
    public bool? RootFolders { get; set; }

    /// <summary>
    /// Filter on repository folder id
    /// </summary>
    public long? FolderId { get; set; }
}
