using NSubstitute;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;

namespace TestBucket.Domain.UnitTests.Issues
{
    /// <summary>
    /// Contains unit tests for the <see cref="SearchIssuesHandler"/> class, verifying the behavior of issue search functionality.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    [Component("Issues")]
    public class SearchIssuesTests
    {
        /// <summary>
        /// Verifies that searching for issues with a specific type returns the correct results.
        /// </summary>
        [Fact]
        public async Task SearchIssues_WithType_ReturnsCorrectResults()
        {
            // Arrange
            var mockIssueManager = Substitute.For<IIssueManager>();
            var handler = new SearchIssuesHandler(mockIssueManager);
            var query = new SearchIssueQuery { Type = "Bug" };
            var request = new SearchIssueRequest(new ClaimsPrincipal(), query);

            mockIssueManager
                .SearchLocalIssuesAsync(Arg.Any<ClaimsPrincipal>(), query, 0, 10)
                .Returns(new PagedResult<LocalIssue>
                {
                    TotalCount = 1,
                    Items = new[] { new LocalIssue { IssueType = "Bug" } }
                });

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Issues);
            Assert.Equal("Bug", response.Issues[0].IssueType);
        }
    }
}