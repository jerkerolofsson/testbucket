using TestBucket.Domain.Progress;

namespace TestBucket.Domain.UnitTests.Progress
{
    /// <summary>
    /// Tests for ProgressManager
    /// </summary>
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    public class ProgressManagerTests
    {
        /// <summary>
        /// Tests that a ProgressTask can be created with a title.
        /// </summary>
        [Fact]
        public void CreateProgressTask_ShouldReturnTaskWithTitle()
        {
            // Arrange
            var progressManager = new ProgressManager();
            var title = "Test Task";

            // Act
            var task = progressManager.CreateProgressTask(title);

            // Assert
            Assert.NotNull(task);
            Assert.Equal(title, task.Title);
        }

        /// <summary>
        /// Tests that observers are notified when a ProgressTask reports status.
        /// </summary>
        [Fact]
        public async Task NotifyAsync_ShouldNotifyObservers()
        {
            // Arrange
            var progressManager = new ProgressManager();
            var observer = new MockProgressObserver();
            progressManager.AddObserver(observer);
            var task = progressManager.CreateProgressTask("Test Task");

            // Act
            await task.ReportStatusAsync("In Progress", 50);

            // Assert
            Assert.Equal("In Progress", observer.LastStatus);
            Assert.Equal(50, observer.LastPercent);
        }

        /// <summary>
        /// Tests that a ProgressTask can be disposed and marked as completed.
        /// </summary>
        [Fact]
        public async Task DisposeAsync_ShouldMarkTaskAsCompleted()
        {
            // Arrange
            var progressManager = new ProgressManager();
            var task = progressManager.CreateProgressTask("Test Task");

            // Act
            await task.DisposeAsync();

            // Assert
            Assert.True(task.Completed);
            Assert.Equal(100, task.Percent);
        }
    }

    /// <summary>
    /// Mock implementation of IProgressObserver for testing purposes.
    /// </summary>
    internal class MockProgressObserver : IProgressObserver
    {
        public string LastStatus { get; private set; } = "";
        public double LastPercent { get; private set; }

        public Task NotifyAsync(ProgressTask progressTask)
        {
            LastStatus = progressTask.Status;
            LastPercent = progressTask.Percent;
            return Task.CompletedTask;
        }
    }
}