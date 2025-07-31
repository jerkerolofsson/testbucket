using TestBucket.Domain.Testing.Specifications.TestRuns;

namespace TestBucket.Domain.UnitTests.Testing.Specifications.TestRuns;

/// <summary>
/// Unit tests for the TestRunSpecifications.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Testing")]
[Feature("Search")]
public class TestRunSpecificationsTests
{
    /// <summary>
    /// Tests that the ExcludeArchivedTestRuns specification matches correctly.
    /// </summary>
    [Fact]
    public void ExcludeArchivedTestRuns_MatchesCorrectly()
    {
        var spec = new ExcludeArchivedTestRuns();
        var match = new TestRun { Name = "TestRun1", Archived = false };
        var noMatch = new TestRun { Name = "TestRun2", Archived = true };

        Assert.True(spec.IsMatch(match));
        Assert.False(spec.IsMatch(noMatch));
    }

    /// <summary>
    /// Tests that the FilterTestRunsByLabFolder specification matches correctly.
    /// </summary>
    [Fact]
    public void FilterTestRunsByLabFolder_MatchesCorrectly()
    {
        var folderId = 123;
        var spec = new FilterTestRunsByLabFolder(folderId);
        var match = new TestRun { Name = "TestRun1", FolderId = folderId };
        var noMatch = new TestRun { Name = "TestRun2", FolderId = 456 };

        Assert.True(spec.IsMatch(match));
        Assert.False(spec.IsMatch(noMatch));
    }

    /// <summary>
    /// Tests that the OnlyArchivedTestRuns specification matches correctly.
    /// </summary>
    [Fact]
    public void OnlyArchivedTestRuns_MatchesCorrectly()
    {
        var spec = new OnlyArchivedTestRuns();
        var match = new TestRun { Name = "TestRun1", Archived = true };
        var noMatch = new TestRun { Name = "TestRun2", Archived = false };

        Assert.True(spec.IsMatch(match));
        Assert.False(spec.IsMatch(noMatch));
    }

    /// <summary>
    /// Tests that the OnlyClosedTestRuns specification matches correctly.
    /// </summary>
    [Fact]
    public void OnlyClosedTestRuns_MatchesCorrectly()
    {
        var spec = new OnlyClosedTestRuns();
        var match = new TestRun { Name = "TestRun1", Open = false };
        var noMatch = new TestRun { Name = "TestRun2", Open = true };

        Assert.True(spec.IsMatch(match));
        Assert.False(spec.IsMatch(noMatch));
    }
}
