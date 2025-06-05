using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Specifications;

namespace TestBucket.Domain.UnitTests.Issues
{
    /// <summary>
    /// Contains unit tests for searching linked issues using various specifications.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Issues")]
    public class SearchLinkedIssueTests
    {
        /// <summary>
        /// Verifies that <see cref="FindLinkedIssueByText"/> returns true when the specified text is contained in the issue title.
        /// </summary>
        [Fact]
        public void FindLinkedIssueByText_ReturnsTrue_WhenTextIsContained()
        {
            var issue = new LinkedIssue { Title = "This is a bug" };
            var spec = new FindLinkedIssueByText("bug");
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLinkedIssueByText"/> returns false when the specified text is not contained in the issue title.
        /// </summary>
        [Fact]
        public void FindLinkedIssueByText_ReturnsFalse_WhenTextIsNotContained()
        {
            var issue = new LinkedIssue { Title = "This is a feature" };
            var spec = new FindLinkedIssueByText("bug");
            Assert.False(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLinkedIssueByTestCaseRunId"/> returns true when the test case run ID matches.
        /// </summary>
        [Fact]
        public void FindLinkedIssueByTestCaseRunId_ReturnsTrue_WhenTestCaseRunIdMatches()
        {
            var issue = new LinkedIssue { TestCaseRunId = 123 };
            var spec = new FindLinkedIssueByTestCaseRunId(123);
            Assert.True(spec.IsMatch(issue));
        }

        /// <summary>
        /// Verifies that <see cref="FindLinkedIssueByTestCaseRunId"/> returns false when the test case run ID does not match.
        /// </summary>
        [Fact]
        public void FindLinkedIssueByTestCaseRunId_ReturnsFalse_WhenTestCaseRunIdDoesNotMatch()
        {
            var issue = new LinkedIssue { TestCaseRunId = 456 };
            var spec = new FindLinkedIssueByTestCaseRunId(123);
            Assert.False(spec.IsMatch(issue));
        }
    }
}