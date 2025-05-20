using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Requirements.Search;

namespace TestBucket.Domain.UnitTests.Requirements.Search
{
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Requirements")]
    public class SearchRequiremensQueryParserTests
    {

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

            var request = SearchRequirementQueryParser.Parse("since:4d", [], timeProvider);

            Assert.NotNull(request.CreatedFrom);
            var actualDate = request.CreatedFrom.Value.ToUniversalTime();
            Assert.Equal(expectedDate.Year, actualDate.Year);
            Assert.Equal(expectedDate.Month, actualDate.Month);
            Assert.Equal(expectedDate.Date, actualDate.Date);
            Assert.Equal(expectedDate.Hour, actualDate.Hour);
        }

        [Fact]
        [TestDescription("""
            Verifies that the is keyword in a query maps to RequirementType for the query

            # Steps
            1. Define the query "is:task"
            2. Verify that "task" is assigned to the RequirementType property when parsed
            """)]
        public void Parse_WithIsTask_RequirementTypeIsTask()
        {
            string text = "is:task";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.Text);
            Assert.Equal("task", request.RequirementType);
        }

        [Fact]
        [TestDescription("""
            Verifies that the state keyword in a query maps to RequirementState for the query

            # Steps
            1. Define the query "state:Open"
            2. Verify that "Open" is assigned to the RequirementState property when parsed
            """)]
        public void Parse_WithStateOpen_TextIsNullTypeIsIssue()
        {
            string text = "state:Open";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.Text);
            Assert.Equal("Open", request.RequirementState);
        }
    }
}
