using TestBucket.Domain.Shared;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.UnitTests.Testing.Search;

/// <summary>
/// Tests that verify the filter FilterTestCasesByReviewAssignedTo
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Testing")]
[Feature("Review")]
[Feature("Search")]
public class FilterTestCasesByReviewAssignedToTests
{
    /// <summary>
    /// Verifies that a FilterTestCasesByReviewAssignedTo filter returns true when trying to match a test case where the ReviewAssignedTo is null
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-002")]
    public void Filter_WithTestCaseFilterSpecificationBuilderTests_When_TestCaseReviewAssignedToIsNull_NoMatch()
    {
        var testCase = new TestCase { Name = "test1", ReviewAssignedTo = null };
        var filter = new FilterTestCasesByReviewAssignedTo("admin@admin.com");

        var expression = filter.Expression.Compile();
        var isMatch = expression(testCase);

        Assert.False(isMatch);
    }

    /// <summary>
    /// Verifies that a FilterTestCasesByReviewAssignedTo filter returns true when trying to match a test case where the ReviewAssignedTo is an empty list
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-002")]
    public void Filter_WithTestCaseFilterSpecificationBuilderTests_When_TestCaseReviewAssignedToIsEmpty_NoMatch()
    {
        var testCase = new TestCase { Name = "test1", ReviewAssignedTo = [] };
        var filter = new FilterTestCasesByReviewAssignedTo("admin@admin.com");

        var expression = filter.Expression.Compile();
        var isMatch = expression(testCase);

        Assert.False(isMatch);
    }

    /// <summary>
    /// Verifies that a FilterTestCasesByReviewAssignedTo filter returns true when trying to match a test case where the ReviewAssignedTo contains a user, but not the same as the filter
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-002")]
    public void Filter_WithTestCaseFilterSpecificationBuilderTests_When_TestCaseReviewAssignedToIsOtherUser_NoMatch()
    {
        var testCase = new TestCase { Name = "test1", ReviewAssignedTo = [new AssignedReviewer { Role = "reviewer", UserName = "admin2@admin.com" }] };
        var filter = new FilterTestCasesByReviewAssignedTo("admin@admin.com");

        var expression = filter.Expression.Compile();
        var isMatch = expression(testCase);

        Assert.False(isMatch);
    }

    /// <summary>
    /// Verifies that a FilterTestCasesByReviewAssignedTo filter returns true when trying to match a test case where the ReviewAssignedTo contains a single user (and it is a match)
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-002")]
    public void Filter_WithTestCaseFilterSpecificationBuilderTests_When_TestCaseReviewAssignedIsSame_IsMatch()
    {
        var testCase = new TestCase { Name = "test1", ReviewAssignedTo = [new AssignedReviewer { Role = "reviewer", UserName = "admin@admin.com" }] };
        var filter = new FilterTestCasesByReviewAssignedTo("admin@admin.com");

        var expression = filter.Expression.Compile();
        var isMatch = expression(testCase);

        Assert.True(isMatch);
    }
    /// <summary>
    /// Verifies that a FilterTestCasesByReviewAssignedTo filter returns true when trying to match a test case where the ReviewAssignedTo contains two user, the second is a match
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-REVIEW-002")]
    public void Filter_WithTestCaseFilterSpecificationBuilderTests_When_TestCaseReviewAssignedHasTwoUsersAndSecondIsMatch_IsMatch()
    {
        var testCase = new TestCase { Name = "test1", ReviewAssignedTo = 
            [
                new AssignedReviewer { Role = "reviewer", UserName = "admin2@admin.com" },
                new AssignedReviewer { Role = "reviewer", UserName = "admin@admin.com" }
            ] };
        var filter = new FilterTestCasesByReviewAssignedTo("admin@admin.com");

        var expression = filter.Expression.Compile();
        var isMatch = expression(testCase);

        Assert.True(isMatch);
    }
}
