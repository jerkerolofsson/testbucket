using TestBucket.Contracts;
using TestBucket.Domain.Testing.Specifications.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.UnitTests.Testing.TestRuns;

/// <summary>
/// Contains unit tests for <see cref="TestRunFilterSpecificationBuilder"/>.
/// Verifies that filter specifications are correctly built from <see cref="SearchTestRunQuery"/>.
/// </summary>
public class TestRunFilterSpecificationBuilderTests
{
    /// <summary>
    /// Ensures that <see cref="TestRunFilterSpecificationBuilder.From(SearchTestRunQuery)"/> returns an empty list when the query is empty.
    /// </summary>
    [Fact]
    public void From_ReturnsEmptyList_WhenQueryIsEmpty()
    {
        var query = new SearchTestRunQuery();
        var specs = TestRunFilterSpecificationBuilder.From(query);
        Assert.Empty(specs);
    }

    /// <summary>
    /// Ensures that a filter specification is added for string fields present in the query.
    /// </summary>
    [Fact]
    public void From_AddsFilterTestRunsByStringField_WhenStringFieldIsPresent()
    {
        var query = new SearchTestRunQuery
        {
            Fields = new List<FieldFilter>
            {
                new FieldFilter { FilterDefinitionId = 1, Name = "Field1", StringValue = "abc" }
            }
        };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        // Dummy test run object with matching field
        var testRun = new TestRun
        {
            TestRunFields = new List<TestRunField>
            {
                new TestRunField { StringValue = "abc", FieldDefinitionId = 1, TestRunId = 0 }
            },
            Name = "run1"
        };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that a filter specification is added for boolean fields present in the query.
    /// </summary>
    [Fact]
    public void From_AddsFilterTestRunsByBooleanField_WhenBooleanFieldIsPresent()
    {
        var query = new SearchTestRunQuery
        {
            Fields = new List<FieldFilter>
            {
                new FieldFilter { FilterDefinitionId = 2, Name = "Field2", BooleanValue = true }
            }
        };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun
        {
            TestRunFields = new List<TestRunField>
            {
                new TestRunField { BooleanValue = true, FieldDefinitionId = 2, TestRunId = 0 }
            },
            Name = "run1"
        };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that a filter specification is added for the lab folder when FolderId is present in the query.
    /// </summary>
    [Fact]
    public void From_AddsFilterTestRunsByLabFolder_WhenFolderIdIsPresent()
    {
        var query = new SearchTestRunQuery { FolderId = 42 };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun { FolderId = 42, Name = "run1" };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that a filter specification is added for the test run ID when TestRunId is present in the query.
    /// </summary>
    [Fact]
    public void From_AddsFilterTestRunsById_WhenTestRunIdIsPresent()
    {
        var query = new SearchTestRunQuery { TestRunId = 99 };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun { Id = 99, Name = "run1" };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that only archived test runs are included when Archived is true in the query.
    /// </summary>
    [Fact]
    public void From_AddsOnlyArchivedTestRuns_WhenArchivedIsTrue()
    {
        var query = new SearchTestRunQuery { Archived = true };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun { Archived = true, Name = "run1" };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that archived test runs are excluded when Archived is false in the query.
    /// </summary>
    [Fact]
    public void From_AddsExcludeArchivedTestRuns_WhenArchivedIsFalse()
    {
        var query = new SearchTestRunQuery { Archived = false };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun { Archived = false, Name = "run1" };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that only open test runs are included when Open is true in the query.
    /// </summary>
    [Fact]
    public void From_AddsOnlyOpenTestRuns_WhenOpenIsTrue()
    {
        var query = new SearchTestRunQuery { Open = true };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun { Open = true, Name = "run1" };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }

    /// <summary>
    /// Ensures that only closed test runs are included when Open is false in the query.
    /// </summary>
    [Fact]
    public void From_AddsOnlyClosedTestRuns_WhenOpenIsFalse()
    {
        var query = new SearchTestRunQuery { Open = false };
        var specs = TestRunFilterSpecificationBuilder.From(query);

        var testRun = new TestRun { Open = false, Name = "run1" };

        Assert.Contains(specs, s => s.IsMatch(testRun));
    }
}