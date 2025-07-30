using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Requests;
using Xunit;
using TestBucket.Domain.Issues;

namespace TestBucket.Domain.UnitTests.Issues
{
    /// <summary>
    /// Unit tests for <see cref="CloseIssueRequestHandler"/> verifying the behavior of closing issues.
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Component("Issues")]
    public class CloseIssueRequestTests
    {
        /// <summary>
        /// Verifies that <see cref="CloseIssueRequestHandler.Handle"/> sets the issue state to closed and updates it.
        /// </summary>
        [Fact]
        public async Task Handle_SetsIssueStateToClosedAndUpdatesIssue()
        {
            // Arrange
            var mockIssueManager = Substitute.For<IIssueManager>();
            var mockFieldManager = Substitute.For<IFieldManager>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();

            var handler = new CloseIssueRequestHandler(mockIssueManager, mockFieldManager, mockFieldDefinitionManager);

            var principal = new ClaimsPrincipal();
            var issue = new LocalIssue { Id = 1, MappedState = MappedIssueState.Open, State = IssueStates.Open };
            var request = new CloseIssueRequest(principal, issue);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(MappedIssueState.Closed, issue.MappedState);
            Assert.Equal(IssueStates.Closed, issue.State);
            await mockIssueManager.Received(1).UpdateLocalIssueAsync(principal, issue);
        }

        /// <summary>
        /// Verifies that <see cref="CloseIssueRequestHandler.Handle"/> sets the commit SHA when provided.
        /// </summary>
        [Fact]
        public async Task Handle_SetsCommitShaWhenProvided()
        {
            // Arrange
            var mockIssueManager = Substitute.For<IIssueManager>();
            var mockFieldManager = Substitute.For<IFieldManager>();
            var mockFieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();

            var handler = new CloseIssueRequestHandler(mockIssueManager, mockFieldManager, mockFieldDefinitionManager);

            var principal = new ClaimsPrincipal();
            var issue = new LocalIssue { Id = 1, TestProjectId = 100, MappedState = MappedIssueState.Open, State = IssueStates.Open };
            var commitSha = "abc123";
            var request = new CloseIssueRequest(principal, issue, commitSha);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            await mockFieldManager.Received(1).SetIssueFieldAsync(principal, 100, 1, Traits.Core.TraitType.Commit, commitSha);
        }
    }
}
