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
        /// 1. Define the query "state:Completed"
        /// 2. Verify that "Completed" is assigned to the <c>RequirementState</c> property when parsed.
        /// </remarks>
        [Fact]
       
        public void Parse_WithStateCompleted_TextIsNullTypeIsIssue()
        {
            string text = "state:Completed";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.Text);
            Assert.Equal("Completed", request.RequirementState);
        }

        /// <summary>
        /// Verifies that the parser correctly sets <c>IsOpen</c> to <c>true</c> when the query contains "open:yes".
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query "open:yes"
        /// 2. Verify that <c>IsOpen</c> is set to <c>true</c> and all other properties are null.
        /// </remarks>
        [Fact]
        public void Parse_WithOpenYes_IsOpenTrue()
        {
            string text = "open:yes";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.True(request.IsOpen);
            Assert.Null(request.RequirementState);
            Assert.Null(request.RequirementType);
            Assert.Null(request.Text);
        }

        /// <summary>
        /// Verifies that the parser correctly sets <c>IsOpen</c> to <c>false</c> when the query contains "open:no".
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query "open:no"
        /// 2. Verify that <c>IsOpen</c> is set to <c>false</c> and all other properties are null.
        /// </remarks>
        [Fact]
        public void Parse_WithOpenNo_IsOpenFalse()
        {
            string text = "open:no";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.False(request.IsOpen);
            Assert.Null(request.RequirementState);
            Assert.Null(request.RequirementType);
            Assert.Null(request.Text);
        }

        /// <summary>
        /// Verifies that the "assigned-to" keyword in a query maps to the <c>AssignedTo</c> property.
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query "assigned-to:alice"
        /// 2. Verify that "alice" is assigned to the <c>AssignedTo</c> property when parsed.
        /// </remarks>
        [Fact]
        public void Parse_WithAssignedToAlice_AssignedToIsAlice()
        {
            string text = "assigned-to:alice";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.Text);
            Assert.Equal("alice", request.AssignedTo);
        }

        /// <summary>
        /// Verifies that the parser leaves <c>AssignedTo</c> as <c>null</c> when the "assigned-to" keyword is not present.
        /// </summary>
        /// <remarks>
        /// Steps:
        /// 1. Define the query "is:task"
        /// 2. Verify that <c>AssignedTo</c> is <c>null</c> when parsed.
        /// </remarks>
        [Fact]
        public void Parse_WithoutAssignedTo_AssignedToIsNull()
        {
            string text = "is:task";
            var request = SearchRequirementQueryParser.Parse(text, []);
            Assert.Null(request.AssignedTo);
        }
    }
}
