using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.UnitTests.Testing.TestCaseRuns
{
    /// <summary>
    /// Contains unit tests for the <see cref="SearchTestCaseRunQuery"/> class, 
    /// verifying correct query string conversion and parsing behavior.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [FunctionalTest]
    [Component("Testing")]
    [Feature("Search")]
    public class SearchTestCaseRunQueryTests
    {
        /// <summary>
        /// Tests that <see cref="SearchTestCaseRunQuery.ToQueryString"/> and <see cref="SearchTestCaseRunQuery.FromUrl"/> 
        /// correctly handle the <c>Unassigned</c> property set to <c>false</c>.
        /// </summary>
        [Fact]
        public void ConvertQuery_WithUnassignedFalse()
        {
            // Arrange
            SearchTestCaseRunQuery query = new SearchTestCaseRunQuery { Unassigned = false };
            var queryString = query.ToQueryString();

            // Act
            var query2 = SearchTestCaseRunQuery.FromUrl([], queryString);

            Assert.Equal(query.Unassigned, query2.Unassigned);
        }

        /// <summary>
        /// Tests that <see cref="SearchTestCaseRunQuery.ToQueryString"/> and <see cref="SearchTestCaseRunQuery.FromUrl"/> 
        /// correctly handle the <c>Unassigned</c> property set to <c>true</c>.
        /// </summary>
        [Fact]
        public void ConvertQuery_WithUnassignedTrue()
        {
            // Arrange
            SearchTestCaseRunQuery query = new SearchTestCaseRunQuery { Unassigned = true };
            var queryString = query.ToQueryString();

            // Act
            var query2 = SearchTestCaseRunQuery.FromUrl([], queryString);

            Assert.Equal(query.Unassigned, query2.Unassigned);
        }

        /// <summary>
        /// Tests that <see cref="SearchTestCaseRunQuery.ToQueryString"/> and <see cref="SearchTestCaseRunQuery.FromUrl"/> 
        /// correctly handle the <c>Result</c> and <c>State</c> properties.
        /// </summary>
        [Fact]
        public void ConvertQuery_WithResultAndState()
        {
            // Arrange
            SearchTestCaseRunQuery query = new SearchTestCaseRunQuery { Result = Contracts.Testing.Models.TestResult.Passed, State = "asd" };
            var queryString = query.ToQueryString();

            // Act
            var query2 = SearchTestCaseRunQuery.FromUrl([], queryString);

            Assert.Equal(query.Result, query2.Result);
            Assert.Equal(query.State, query2.State);
        }

        /// <summary>
        /// Tests that <see cref="SearchTestCaseRunQuery.ToQueryString"/> and <see cref="SearchTestCaseRunQuery.FromUrl"/> 
        /// correctly handle the <c>TeamId</c> property.
        /// </summary>
        [Fact]
        public void ConvertQuery_WithTeamId()
        {
            // Arrange
            SearchTestCaseRunQuery query = new SearchTestCaseRunQuery { TeamId = 3123 };
            var queryString = query.ToQueryString();

            // Act
            var query2 = SearchTestCaseRunQuery.FromUrl([], queryString);

            Assert.Equal(query.TeamId, query2.TeamId);
        }

        /// <summary>
        /// Tests that <see cref="SearchTestCaseRunQuery.ToQueryString"/> and <see cref="SearchTestCaseRunQuery.FromUrl"/> 
        /// correctly handle the <c>TestRunId</c> property.
        /// </summary>
        [Fact]
        public void ConvertQuery_WithRunId()
        {
            // Arrange
            SearchTestCaseRunQuery query = new SearchTestCaseRunQuery { TestRunId = 123 };
            var queryString = query.ToQueryString();

            // Act
            var query2 = SearchTestCaseRunQuery.FromUrl([], queryString);

            Assert.Equal(query.TestRunId, query2.TestRunId);
        }

        /// <summary>
        /// Tests that <see cref="SearchTestCaseRunQuery.ToQueryString"/> and <see cref="SearchTestCaseRunQuery.FromUrl"/> 
        /// correctly handle the <c>Result</c> property.
        /// </summary>
        [Fact]
        public void ConvertQuery_WithResult()
        {
            // Arrange
            SearchTestCaseRunQuery query = new SearchTestCaseRunQuery { Result = Contracts.Testing.Models.TestResult.Passed };
            var queryString = query.ToQueryString();

            // Act
            var query2 = SearchTestCaseRunQuery.FromUrl([], queryString);

            Assert.Equal(query.Result, query2.Result);
        }
    }
}