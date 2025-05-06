namespace TestBucket.Components.Tests.TestCases.Models;

public class TestCaseList
{
    /// <summary>
    /// A query that defines the tests
    /// </summary>
    public SearchTestQuery? Query { get; set; }

    /// <summary>
    /// Specific test cases 
    /// </summary>
    public List<long> TestCaseIds { get; } = [];
}
