using TestBucket.Domain.Search.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.UnitTests.Search
{
    /// <summary>
    /// Contains unit tests for the <see cref="SearchResult"/> class, verifying its construction and property assignments.
    /// </summary>
    [Feature("Search")]
    [UnitTest]
    [Component("Search")]
    [EnrichedTest]
    [FunctionalTest]
    public class SearchResultTests
    {
        /// <summary>
        /// Tests that the <see cref="SearchResult"/> constructor correctly sets the <see cref="SearchResult.TestCase"/> and <see cref="SearchResult.Text"/> properties
        /// when initialized with a <see cref="TestCase"/> instance.
        /// </summary>
        [Fact]
        public void Constructor_WithTestCase_SetsTestCaseAndText()
        {
            var testCase = new TestCase { Name = "Test Case 1" };
            var result = new SearchResult(testCase);

            Assert.Equal(testCase, result.TestCase);
            Assert.Null(result.Requirement);
            Assert.Equal("Test Case 1", result.Text);
            Assert.Equal("Test Case 1", result.ToString());
        }

        /// <summary>
        /// Tests that the <see cref="SearchResult"/> constructor correctly sets the <see cref="SearchResult.Requirement"/> and <see cref="SearchResult.Text"/> properties
        /// when initialized with a <see cref="Requirement"/> instance.
        /// </summary>
        [Fact]
        public void Constructor_WithRequirement_SetsRequirementAndText()
        {
            var requirement = new Requirement { Name = "Requirement A" };
            var result = new SearchResult(requirement);

            Assert.Equal(requirement, result.Requirement);
            Assert.Null(result.TestCase);
            Assert.Equal("Requirement A", result.Text);
            Assert.Equal("Requirement A", result.ToString());
        }
    }
}