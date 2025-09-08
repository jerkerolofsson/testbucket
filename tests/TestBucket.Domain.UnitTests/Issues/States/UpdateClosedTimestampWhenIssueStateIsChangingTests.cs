using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Events;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.States;

namespace TestBucket.Domain.UnitTests.Issues.States;

/// <summary>
/// Unit tests for the <see cref="UpdateClosedTimestampWhenIssueStateIsChanging"/> class.
/// Verifies that the closed timestamp is updated correctly when the issue state changes.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Issues")]
public class UpdateClosedTimestampWhenIssueStateIsChangingTests
{
    /// <summary>
    /// Tests that the closed timestamp is updated when the issue state is set to "Closed".
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateClosedTimestamp_WhenStateIsClosed()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 8, 22, 33, 16, TimeSpan.Zero));
        var handler = new UpdateClosedTimestampWhenIssueStateIsChanging(timeProvider);
        var issue = new LocalIssue { MappedState = MappedIssueState.Closed, Closed = DateTimeOffset.MinValue };
        var principal = Impersonation.Impersonate("tenant-1");
        var notification = new IssueStateChangingNotification(principal, issue, "Open");

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        Assert.Equal(timeProvider.GetUtcNow(), issue.Closed.Value);
    }

    /// <summary>
    /// Tests that the closed timestamp is updated when the issue state is set to "Canceled".
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateClosedTimestamp_WhenStateIsCanceled()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 8, 22, 33, 16, TimeSpan.Zero));
        var handler = new UpdateClosedTimestampWhenIssueStateIsChanging(timeProvider);
        var issue = new LocalIssue { MappedState = MappedIssueState.Canceled, Closed = DateTimeOffset.MinValue };
        var principal = Impersonation.Impersonate("tenant-1");
        var notification = new IssueStateChangingNotification(principal, issue, "Open");

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        Assert.Equal(timeProvider.GetUtcNow(), issue.Closed.Value);
    }

    /// <summary>
    /// Tests that the closed timestamp is not updated when the issue state is neither "Closed" nor "Canceled".
    /// </summary>
    [Fact]
    public async Task Handle_ShouldNotUpdateClosedTimestamp_WhenStateIsNotClosedOrCanceled()
    {
        // Arrange
        var handler = new UpdateClosedTimestampWhenIssueStateIsChanging(TimeProvider.System);
        var issue = new LocalIssue { MappedState = MappedIssueState.Open, Closed = DateTimeOffset.MinValue };
        var principal = Impersonation.Impersonate("tenant-1");
        var notification = new IssueStateChangingNotification(principal, issue, "Open");

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        Assert.Equal(DateTimeOffset.MinValue, issue.Closed);
    }
}