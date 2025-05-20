using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.UnitTests.Issues
{
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Issues")]
    public class SearchIssueRequestParserTests
    {
        [Theory]
        [InlineData("Hello World")]
        [InlineData("HelloWorld")]
        [InlineData("stateopen")]
        public void Parse_WithNoKeywords_PlainText(string text)
        {
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Equal(text, request.Text);
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

            var request = SearchIssueRequestParser.Parse(1, "since:4d", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        [Fact]
        public void Parse_WithIsIncident_TextIsNullTypeIsIssue()
        {
            string text = "is:incident";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("incident", request.Type);
        }

        [Fact]
        public void Parse_WithIsIssue_TextIsNullTypeIsIssue()
        {
            string text = "is:issue";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("issue", request.Type);
        }
        [Fact]
        public void Parse_WithOriginGithub_TextIsNullOriginIsGithub()
        {
            string text = "origin:Github";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("Github", request.ExternalSystemName);
        }


        [Fact]
        [TestDescription("""
            This test verifies that the query: team-id: <integer> can be parsed correctly
            """)]
        public void Parse_WithTeamId_ParsedCorrectly()
        {
            var request = SearchIssueRequestParser.Parse(1, "team-id:123", []);
            Assert.Equal(123, request.TeamId);
        }

        [Fact]
        [TestDescription("""
            Verifies that the query "state:open" can be parsed
            """)]
        public void Parse_WithStateOpen_TextIsNullStateIsOpen()
        {
            string text = "state:open";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("open", request.State);
        }

        [Fact]
        [TestDescription("""
            This test verifies that the unstructured part of a query is parsed as a text search

            # Steps
            1. Define a query that has a structured part and an unstructed part: "state:open Hello" (state:open is structured and Hello is unstructured)
            2. Verify that the result has the State property set to "open" and the Text property set to "Hello"
            """)]
        public void Parse_WithStateOpenAndText_TextIsRemainderStateIsOpen()
        {
            string text = "state:open Hello";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Equal("Hello", request.Text);
            Assert.Equal("open", request.State);
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

            var request = SearchIssueRequestParser.Parse(1, text, fieldDefinitions);
            Assert.Single(request.Fields);
            Assert.Equal("1.0", request.Fields[0].StringValue);
            Assert.Equal(123, request.Fields[0].FilterDefinitionId);
        }
    }
}
