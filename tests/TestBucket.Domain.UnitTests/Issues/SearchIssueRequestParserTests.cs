using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.UnitTests.Issues
{
    /// <summary>
    /// Contains unit tests for the <see cref="SearchIssueRequestParser"/> class, verifying correct parsing of issue search queries.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Issues")]
    public class SearchIssueRequestParserTests
    {
        /// <summary>
        /// Verifies that plain text queries without keywords are parsed as text.
        /// </summary>
        /// <param name="text">The input query text.</param>
        [Theory]
        [InlineData("Hello World")]
        [InlineData("HelloWorld")]
        [InlineData("stateopen")]
        public void Parse_WithNoKeywords_PlainText(string text)
        {
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Equal(text, request.Text);
        }

        /// <summary>
        /// Verifies that a "since" query with days sets the <c>CreatedFrom</c> property correctly.
        /// </summary>
        [Fact]
       
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

        /// <summary>
        /// Verifies that the "is:incident" query sets <c>Type</c> to "incident" and <c>Text</c> to null.
        /// </summary>
        [Fact]
        public void Parse_WithIsIncident_TextIsNullTypeIsIssue()
        {
            string text = "is:incident";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("incident", request.Type);
        }

        /// <summary>
        /// Verifies that the "is:issue" query sets <c>Type</c> to "issue" and <c>Text</c> to null.
        /// </summary>
        [Fact]
        public void Parse_WithIsIssue_TextIsNullTypeIsIssue()
        {
            string text = "is:issue";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("issue", request.Type);
        }

        /// <summary>
        /// Verifies that the "origin:Github" query sets <c>ExternalSystemName</c> to "Github" and <c>Text</c> to null.
        /// </summary>
        [Fact]
        public void Parse_WithOriginGithub_TextIsNullOriginIsGithub()
        {
            string text = "origin:Github";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("Github", request.ExternalSystemName);
        }

        /// <summary>
        /// Verifies that the "team-id:&lt;integer&gt;" query sets the <c>TeamId</c> property correctly.
        /// </summary>
        [Fact]
      
        public void Parse_WithTeamId_ParsedCorrectly()
        {
            var request = SearchIssueRequestParser.Parse(1, "team-id:123", []);
            Assert.Equal(123, request.TeamId);
        }

        /// <summary>
        /// Verifies that the "state:open" query sets <c>State</c> to "open" and <c>Text</c> to null.
        /// </summary>
        [Fact]
      
        public void Parse_WithStateOpen_TextIsNullStateIsOpen()
        {
            string text = "state:open";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("open", request.State);
        }

        /// <summary>
        /// Verifies that a query with both structured and unstructured parts parses the remainder as text.
        /// </summary>
        [Fact]
       
        public void Parse_WithStateOpenAndText_TextIsRemainderStateIsOpen()
        {
            string text = "state:open Hello";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Equal("Hello", request.Text);
            Assert.Equal("open", request.State);
        }

        /// <summary>
        /// Verifies that a field query (e.g., "milestone:1.0") is parsed and added as a field filter.
        /// </summary>
        [Fact]
        public void Parse_WithField_AddedAsField()
        {
            string text = "milestone:1.0";
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition() { Name = "Milestone", Id = 123 }];

            var request = SearchIssueRequestParser.Parse(1, text, fieldDefinitions);
            Assert.Single(request.Fields);
            Assert.Equal("1.0", request.Fields[0].StringValue);
            Assert.Equal(123, request.Fields[0].FilterDefinitionId);
        }
    }
}