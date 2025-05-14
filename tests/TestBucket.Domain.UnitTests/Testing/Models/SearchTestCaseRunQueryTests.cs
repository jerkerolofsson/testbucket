using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.UnitTests.Testing.Models
{
    [EnrichedTest]
    [UnitTest]
    public class SearchTestCaseRunQueryTests
    {
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
