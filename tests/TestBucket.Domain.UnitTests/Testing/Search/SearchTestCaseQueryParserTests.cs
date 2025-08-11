using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.UnitTests.Testing.Search
{
    /// <summary>
    /// Contains unit tests for <see cref="SearchTestCaseQueryParser"/> to verify parsing of test case search queries.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Testing")]
    public class SearchTestCaseQueryParserTests
    {
        /// <summary>
        /// Verifies that plain text queries without keywords are parsed as text search.
        /// </summary>
        /// <param name="text">The input query string.</param>
        [Theory]
        [InlineData("Hello World")]
        [InlineData("HelloWorld")]
        [InlineData("stateopen")]
        public void Parse_WithNoKeywords_PlainText(string text)
        {
            var request = SearchTestCaseQueryParser.Parse(text, []);
            Assert.Equal(text, request.Text);
        }



        /// <summary>
        /// Verifies that a query with "review-assigned-to:admin@admin.com" sets the ReviewAssignedTo property
        /// </summary>
        [Fact]
        [CoveredRequirement("TB-REVIEW-002")]
        public void Parse_WithReviewAssignedTop_ParsedCorrectly()
        {
            var currentDate = new DateTimeOffset(2025, 5, 20, 12, 0, 0, TimeSpan.Zero);
            var expectedDate = currentDate.Subtract(TimeSpan.FromHours(5));
            var timeProvider = new FakeTimeProvider(currentDate);

            var request = SearchTestCaseQueryParser.Parse("review-assigned-to:admin@admin.com", [], timeProvider);

            Assert.NotNull(request.ReviewAssignedTo);
            Assert.Equal("admin@admin.com", request.ReviewAssignedTo);
        }

        /// <summary>
        /// Verifies that a query with "since:5h" sets the <c>CreatedFrom</c> property to 5 hours before the current time.
        /// </summary>
        [Fact]
        public void Parse_WithSince5h_ParsedCorrectly()
        {
            var currentDate = new DateTimeOffset(2025, 5, 20, 12, 0, 0, TimeSpan.Zero);
            var expectedDate = currentDate.Subtract(TimeSpan.FromHours(5));
            var timeProvider = new FakeTimeProvider(currentDate);

            var request = SearchTestCaseQueryParser.Parse("since:5h", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        /// <summary>
        /// Verifies that a query with "since:4d" sets the <c>CreatedFrom</c> property to 4 days before the current time.
        /// </summary>
        [Fact]
      
        public void Parse_WithSince4d_ParsedCorrectly()
        {
            var currentDate = new DateTimeOffset(2025, 5, 20, 12, 0, 0, TimeSpan.Zero);
            var expectedDate = currentDate.Subtract(TimeSpan.FromDays(4));
            var timeProvider = new FakeTimeProvider(currentDate);

            var request = SearchTestCaseQueryParser.Parse("since:4d", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        /// <summary>
        /// Verifies that a query with "since:20m" sets the <c>CreatedFrom</c> property to 20 minutes before the current time.
        /// </summary>
        [Fact]
       
        public void Parse_WithSince20m_ParsedCorrectly()
        {
            var currentDate = new DateTimeOffset(2025, 5, 20, 12, 0, 0, TimeSpan.Zero);
            var expectedDate = currentDate.Subtract(TimeSpan.FromMinutes(20));
            var timeProvider = new FakeTimeProvider(currentDate);

            var request = SearchTestCaseQueryParser.Parse("since:20m", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        /// <summary>
        /// Verifies that a query with "since:120s" sets the <c>CreatedFrom</c> property to 120 seconds before the current time.
        /// </summary>
        [Fact]
       
        public void Parse_WithSince120s_ParsedCorrectly()
        {
            var currentDate = new DateTimeOffset(2025, 5, 20, 12, 0, 0, TimeSpan.Zero);
            var expectedDate = currentDate.Subtract(TimeSpan.FromSeconds(120));
            var timeProvider = new FakeTimeProvider(currentDate);

            var request = SearchTestCaseQueryParser.Parse("since:120s", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        /// <summary>
        /// Verifies that a query with "since:2w" sets the <c>CreatedFrom</c> property to 2 weeks before the current time.
        /// </summary>
        [Fact]
       
        public void Parse_WithSince2w_ParsedCorrectly()
        {
            var currentDate = new DateTimeOffset(2025, 5, 20, 12, 0, 0, TimeSpan.Zero);
            var expectedDate = currentDate.Subtract(TimeSpan.FromDays(14));
            var timeProvider = new FakeTimeProvider(currentDate);

            var request = SearchTestCaseQueryParser.Parse("since:2w", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        /// <summary>
        /// Verifies that a query with "from:yyyy-MM-dd" sets the <c>CreatedFrom</c> property to the specified date.
        /// </summary>
        [Fact]
       
        public void Parse_WithFrom_ParsedCorrectly()
        {
            var date = new DateTimeOffset(2025, 5, 20, 0, 0, 0, TimeSpan.Zero);

            var request = SearchTestCaseQueryParser.Parse("from:2025-05-20", []);
            Assert.NotNull(request.CreatedFrom);
            Assert.Equal(2025, request.CreatedFrom.Value.Year);
            Assert.Equal(5, request.CreatedFrom.Value.Month);
            Assert.Equal(20, request.CreatedFrom.Value.Day);
        }

        /// <summary>
        /// Verifies that a query with "team-id:&lt;integer&gt;" sets the <c>TeamId</c> property.
        /// </summary>
        [Fact]
        
        public void Parse_WithTeamId_ParsedCorrectly()
        {
            var request = SearchTestCaseQueryParser.Parse("team-id:123", []);
            Assert.Equal(123, request.TeamId);
        }

        /// <summary>
        /// Verifies that the unstructured part of a query is parsed as a text search and structured part is parsed as a filter.
        /// </summary>
        [Fact]
       
        public void Parse_WithIsManualAndText_TextIsRemainderTypeIsManual()
        {
            string text = "is:manual Hello";
            var request = SearchTestCaseQueryParser.Parse(text, []);
            Assert.Equal("Hello", request.Text);
            Assert.Equal(Contracts.Testing.Models.TestExecutionType.Manual, request.TestExecutionType);
        }

        /// <summary>
        /// Verifies that a query defining a field (e.g., "milestone:1.0") is parsed and mapped to the correct field definition.
        /// </summary>
        [Fact]
        public void Parse_WithField_AddedAsField()
        {
            string text = "milestone:1.0";
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition() { Name = "Milestone", Id = 123 }];

            var request = SearchTestCaseQueryParser.Parse(text, fieldDefinitions);
            Assert.Single(request.Fields);
            Assert.Equal("1.0", request.Fields[0].StringValue);
            Assert.Equal(123, request.Fields[0].FilterDefinitionId);
        }
    }
}