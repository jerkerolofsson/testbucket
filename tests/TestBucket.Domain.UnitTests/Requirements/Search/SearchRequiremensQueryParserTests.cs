using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.Requirements.Search;

namespace TestBucket.Domain.UnitTests.Requirements.Search
{
    /// <summary>
    /// Contains unit tests for the <see cref="SearchRequirementQueryParser"/> class, verifying correct parsing of requirement search queries.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Requirements")]
    public class SearchRequiremensQueryParserTests
    {
        /// <summary>
        /// Verifies that a query with a "since" keyword and days value sets the <c>CreatedFrom</c> property correctly.
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query: "since:4d"
        /// 2. Verify that the <c>CreatedFrom</c> property on the query is set to the current time minus 4 days.
        /// </remarks>
        [Fact]
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

        /// <summary>
        /// Verifies that the "is" keyword in a query maps to the <c>RequirementType</c> property.
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query "is:task"
        /// 2. Verify that "task" is assigned to the <c>RequirementType</c> property when parsed.
        /// </remarks>
        [Fact]
        
        public void Parse_WithIsTask_RequirementTypeIsTask()
        {
            string text = "is:task";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.Text);
            Assert.Equal("task", request.RequirementType);
        }

        /// <summary>
        /// Verifies that the "state" keyword in a query maps to the <c>RequirementState</c> property.
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query "state:Open"
        /// 2. Verify that "Open" is assigned to the <c>RequirementState</c> property when parsed.
        /// </remarks>
        [Fact]
       
        public void Parse_WithStateOpen_TextIsNullTypeIsIssue()
        {
            string text = "state:Open";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.Text);
            Assert.Equal("Open", request.RequirementState);
        }
    }
}
