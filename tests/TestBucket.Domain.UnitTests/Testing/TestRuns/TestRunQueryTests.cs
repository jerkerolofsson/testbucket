using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.UnitTests.Testing.TestRuns;

/// <summary>
/// Tests for SearchTestRunQuery
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Search")]
public class TestRunQueryTests
{
    /// <summary>
    /// Tests that <see cref="SearchTestRunQuery.ToQueryString"/> and <see cref="SearchTestRunQuery.FromUrl"/> 
    /// correctly handle the <c>TestRunId</c> property.
    /// </summary>
    [Fact]
    public void ConvertQuery_WithRunId()
    {
        // Arrange
        var query = new SearchTestRunQuery { TestRunId = 123 };
        var queryString = query.ToQueryString();

        // Act
        var query2 = SearchTestRunQuery.FromUrl([], queryString);

        Assert.Equal(query.TestRunId, query2.TestRunId);
    }   

    /// <summary>
    /// Tests that <see cref="SearchTestRunQuery.ToQueryString"/> and <see cref="SearchTestRunQuery.FromUrl"/> 
    /// correctly handle the <c>FolderId</c> property.
    /// </summary>
    [Fact]
    public void ConvertQuery_WithFolderId()
    {
        // Arrange
        var query = new SearchTestRunQuery { FolderId = 456 };
        var queryString = query.ToQueryString();

        // Act
        var query2 = SearchTestRunQuery.FromUrl([], queryString);

        Assert.Equal(query.FolderId, query2.FolderId);
    }

    /// <summary>
    /// Tests that <see cref="SearchTestRunQuery.ToQueryString"/> and <see cref="SearchTestRunQuery.FromUrl"/> 
    /// correctly handle the <c>Archived</c> property.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConvertQuery_WithArchived(bool archived)
    {
        // Arrange
        var query = new SearchTestRunQuery { Archived = archived };
        var queryString = query.ToQueryString();

        // Act
        var query2 = SearchTestRunQuery.FromUrl([], queryString);

        Assert.Equal(query.Archived, query2.Archived);
    }

    /// <summary>
    /// Tests that <see cref="SearchTestRunQuery.ToQueryString"/> and <see cref="SearchTestRunQuery.FromUrl"/> 
    /// correctly handle the <c>Open</c> property.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConvertQuery_WithOpen(bool open)
    {
        // Arrange
        var query = new SearchTestRunQuery { Open = open };
        var queryString = query.ToQueryString();

        // Act
        var query2 = SearchTestRunQuery.FromUrl([], queryString);

        Assert.Equal(query.Open, query2.Open);
    }

    /// <summary>
    /// Tests that <see cref="SearchTestRunQuery.ToSearchText"/> combines all properties into the search text.
    /// </summary>
    [Fact]
    public void ToSearchText_CombinesAllProperties()
    {
        // Arrange
        var query = new SearchTestRunQuery
        {
            TestRunId = 1,
            FolderId = 2,
            Archived = true,
            Open = false
        };

        // Act
        var searchText = query.ToSearchText();

        // Assert
        Assert.Contains("testrun-id:1", searchText);
        Assert.Contains("folder-id:2", searchText);
        Assert.Contains("archived:yes", searchText);
        Assert.Contains("open:no", searchText);
    }
}
