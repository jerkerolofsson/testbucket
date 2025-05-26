using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    [FunctionalTest]
    public class TestRunManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task AddTestRun_WithProjectIdButNoTeamId_TeamIdIsSet()
        {
            // Arrange
            var run = new TestRun { Name = "ads" + Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId };

            // Act
            await Fixture.Runs.AddAsync(run);

            // Assert 
            Assert.NotNull(run.TeamId);
            Assert.Equal(Fixture.Project.TeamId, run.TeamId);
        }

        [Fact]
        [FunctionalTest]
        public async Task AddTestRun_CreatedDateSet()
        {
            // Arrange
            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero));
            var run = new TestRun { Name = "ads" + Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId };

            // Act
            await Fixture.Runs.AddAsync(run);

            // Assert 
            Assert.Equal(Fixture.App.TimeProvider.GetUtcNow(), run.Created);
        }


        [Fact]
        [FunctionalTest]
        public async Task CloseRun_WithOpenRun_ClosesTheRun()
        {
            // Arrange
            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero));
            var run = new TestRun { Name = "ads" + Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, Open = true };
            await Fixture.Runs.AddAsync(run);

            // Act
            var user = Impersonation.Impersonate(Fixture.App.Tenant);
            await Fixture.App.Mediator.Send(new CloseRunRequest(user, run.Id), TestContext.Current.CancellationToken);

            // Assert 
            TestRun? runFromDb = await Fixture.Runs.GetRunByIdAsync(run.Id);
            Assert.NotNull(runFromDb);
            Assert.False(runFromDb.Open);
        }
    }
}
