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
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Testing")]
    public class SearchTestCaseQueryParserTests
    {
        [Theory]
        [InlineData("Hello World")]
        [InlineData("HelloWorld")]
        [InlineData("stateopen")]
        public void Parse_WithNoKeywords_PlainText(string text)
        {
            var request = SearchTestCaseQueryParser.Parse(text, []);
            Assert.Equal(text, request.Text);
        }

        [Fact]
        [TestDescription("""
            This test verifies that the queries that define a "since" with hours query sets the CreatedFrom property

            # Steps:
            1. Define the query: "since:5h"
            2. Verify that the CreatedFrom property on the query is set to the current time - 5 hours
            """)]
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

        [Fact]
        [TestDescription("""
            This test verifies that the queries that define a "since" with days query sets the CreatedFrom property

            # Steps:
            1. Define the query: "since:4d"
            2. Verify that the CreatedFrom property on the query is set to the current time - 4 days
            """)]
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

        [Fact]
        [TestDescription("""
            This test verifies that the queries that define a "since" with days query sets the CreatedFrom property

            # Steps:
            1. Define the query: "since:20m"
            2. Verify that the CreatedFrom property on the query is set to the current time - 20 minutes
            """)]
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

        [Fact]
        [TestDescription("""
            This test verifies that the queries that define a "since" with days query sets the CreatedFrom property

            # Steps:
            1. Define the query: "since:120s"
            2. Verify that the CreatedFrom property on the query is set to the current time - 120seconds
            """)]
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


        [Fact]
        [TestDescription("""
            This test verifies that the queries that define a "since" with days query sets the CreatedFrom property

            # Steps:
            1. Define the query: "since:120s"
            2. Verify that the CreatedFrom property on the query is set to the current time - 14days
            """)]
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

        [Fact]
        [TestDescription("""
            This test verifies that the queries that define a DateTimeOffset for the From property can be parsed
            from the query: from:yyyy-MM-dd
            """)]
        public void Parse_WithFrom_ParsedCorrectly()
        {
            var date = new DateTimeOffset(2025, 5, 20,0,0,0,TimeSpan.Zero);
            
            var request = SearchTestCaseQueryParser.Parse("from:2025-05-20", []);
            Assert.NotNull(request.CreatedFrom);
            Assert.Equal(2025, request.CreatedFrom.Value.Year);
            Assert.Equal(5, request.CreatedFrom.Value.Month);
            Assert.Equal(20, request.CreatedFrom.Value.Day);
        }

        [Fact]
        [TestDescription("""
            This test verifies that the query: team-id: <integer> can be parsed correctly
            """)]
        public void Parse_WithTeamId_ParsedCorrectly()
        {
            var request = SearchTestCaseQueryParser.Parse("team-id:123", []);
            Assert.Equal(123, request.TeamId);
        }

        [Fact]
        [TestDescription("""
            This test verifies that the unstructured part of a query is parsed as a text search

            # Steps
            1. Define a query that has a structured part and an unstructed part: "is:manual Hello" (is:manual is structured and Hello is unstructured)
            2. Verify that the result has the TestExecutionType property set to "TestExecutionType.Manual" and the Text property set to "Hello"
            """)]
        public void Parse_WithIsManualAndText_TextIsRemainderTypeIsManual()
        {
            string text = "is:manual Hello";
            var request = SearchTestCaseQueryParser.Parse(text, []);
            Assert.Equal("Hello", request.Text);
            Assert.Equal(Contracts.Testing.Models.TestExecutionType.Manual, request.TestExecutionType);
        }

        [Fact]
        [TestDescription("""
            This test verifies that a query that defines a field is parsed correctly

            # Steps
            1. Define the query "milestone:1.0"
            2. Verify that the result contains the value "1.0"
            3. Verify that the result contains the field ID correpsonding to the field definition ID property
            """)]
        public void Parse_WithField_AddedAsField()
        {
            string text = "milestone:1.0";
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition() { Name = "Milestone", Id = 123}];

            var request = SearchTestCaseQueryParser.Parse(text, fieldDefinitions);
            Assert.Single(request.Fields);
            Assert.Equal("1.0", request.Fields[0].StringValue);
            Assert.Equal(123, request.Fields[0].FilterDefinitionId);
        }
    }
}
