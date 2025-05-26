
using Microsoft.Extensions.DependencyInjection;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.IntegrationTests.Fixtures;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    public class TestRunManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task AddTestRun_WithProjectIdButNoTeamId_TeamIdIsSet()
        {
            // Arrange
            var run = new TestRun { Name = "ads" + Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId };

            // Act
            await Fixture.Tests.AddRunAsync(run);

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
            await Fixture.Tests.AddRunAsync(run);

            // Assert 
            Assert.Equal(Fixture.App.TimeProvider.GetUtcNow(), run.Created);
        }
    }
}
