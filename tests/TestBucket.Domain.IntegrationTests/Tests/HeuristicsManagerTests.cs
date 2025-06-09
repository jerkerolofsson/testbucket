using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    /// <summary>
    /// Integration tests for managing heuristics within a project.
    /// </summary>
    [IntegrationTest]
    [EnrichedTest]
    [Feature("Heuristics")]
    [Component("Testing")]
    [FunctionalTest]
    public class HeuristicsManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that default heuristics exist after a new project is created.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task DefaultHeuristicsExists_AfterProjectCreated()
        {
            // Arrange
            var project = await Fixture.CreateProjectAsync();
            try
            {
                // Act
                IReadOnlyList<Heuristic> heuristics = await Fixture.Heuristics.GetHeuristicsAsync(project.Id);

                // Assert 
                Assert.NotEmpty(heuristics);
            }
            finally
            {
                await Fixture.DeleteProjectAsync(project);
            }
        }

        /// <summary>
        /// Ensures that retrieving heuristics for a non-existent project returns an empty list.
        /// </summary>
        [Fact]
        public async Task GetHeuristics_ReturnsEmptyList_ForNonExistentProject()
        {
            // Use a random project id unlikely to exist
            var heuristics = await Fixture.Heuristics.GetHeuristicsAsync(-99999);
            Assert.Empty(heuristics);
        }

        /// <summary>
        /// Tests that a heuristic can be added to a project and is retrievable.
        /// </summary>
        [Fact]
        public async Task CanAddHeuristicToProject()
        {
            var project = await Fixture.CreateProjectAsync();
            try
            {
                // Act
                var newHeuristic = await Fixture.Heuristics.AddHeuristicAsync(project.Id);
                var heuristics = await Fixture.Heuristics.GetHeuristicsAsync(project.Id);

                // Assert
                Assert.Contains(heuristics, h => h.Id == newHeuristic.Id);
            }
            finally
            {
                await Fixture.DeleteProjectAsync(project);
            }
        }

        /// <summary>
        /// Verifies that the created and modified timestamps are set when adding a heuristic.
        /// </summary>
        [Fact]
        public async Task AddHeuristic_CreatedAndModifiedTimeIsSet()
        {
            var project = await Fixture.CreateProjectAsync();
            var createdDate = new DateTimeOffset(2025, 6, 9, 13, 4, 22, TimeSpan.Zero);
            Fixture.App.TimeProvider.SetTime(createdDate);

            try
            {
                // Act
                var newHeuristic = await Fixture.Heuristics.AddHeuristicAsync(project.Id);

                // Assert
                var heuristics = await Fixture.Heuristics.GetHeuristicsAsync(project.Id);
                var heuristic = heuristics.Where(x => x.Id == newHeuristic.Id).FirstOrDefault();
                Assert.NotNull(heuristic);
                Assert.Equal(createdDate, heuristic.Created);
                Assert.Equal(createdDate, heuristic.Modified);
            }
            finally
            {
                await Fixture.DeleteProjectAsync(project);
            }
        }

        /// <summary>
        /// Ensures that updating a heuristic updates the modified timestamp but not the created timestamp.
        /// </summary>
        [Fact]
        public async Task UpdatedHeuristic_ModifiedTimeIsUpdated()
        {
            var project = await Fixture.CreateProjectAsync();
            var createdDate = new DateTimeOffset(2025, 6, 9, 13, 4, 22, TimeSpan.Zero);
            Fixture.App.TimeProvider.SetTime(createdDate);
            var newHeuristic = await Fixture.Heuristics.AddHeuristicAsync(project.Id);

            try
            {
                // Act
                var updatedDate = new DateTimeOffset(2025, 6, 9, 13, 4, 22, TimeSpan.Zero);
                Fixture.App.TimeProvider.SetTime(updatedDate);
                await Fixture.Heuristics.UpdateHeuristicAsync(newHeuristic);

                // Assert
                var heuristics = await Fixture.Heuristics.GetHeuristicsAsync(project.Id);
                var heuristic = heuristics.Where(x => x.Id == newHeuristic.Id).FirstOrDefault();
                Assert.NotNull(heuristic);
                Assert.Equal(createdDate, heuristic.Created);
                Assert.Equal(updatedDate, heuristic.Modified);
            }
            finally
            {
                await Fixture.DeleteProjectAsync(project);
            }
        }
    }
}