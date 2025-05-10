using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;

namespace TestBucket.Domain.IntegrationTests.Milestones
{
    [IntegrationTest]
    [FunctionalTest]
    [Feature("Milestones")]
    public class ManageMilestoneTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that a milestone's description can be updated
            """)]
        public async Task UpdateMilestoneDescription_Success()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var principal = Fixture.App.SiteAdministrator;
            await DeleteAllProjectMilestonesAsync(principal);

            var ms = new Milestone() { Title = "1.0", TestProjectId = Fixture.ProjectId };
            await manager.AddMilestoneAsync(principal, ms);

            // Act
            ms.Description = "Changed description";
            await manager.UpdateMilestoneAsync(principal, ms);

            // Assert
            var milestones = await manager.GetMilestonesAsync(principal, Fixture.ProjectId);
            Assert.Single(milestones);
            Assert.Equal(ms.Description, milestones[0].Description);
        }

        [Fact]
        [TestDescription("""
            Verifies that a milestone can be created and deleted
            """)]
        public async Task CreateDeleteMilestone_Success()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var principal = Fixture.App.SiteAdministrator;
            await DeleteAllProjectMilestonesAsync(principal);

            // Act
            await manager.AddMilestoneAsync(principal, new Milestone() {  Title = "1.0", TestProjectId = Fixture.ProjectId });

            // Assert
            var milestones = await manager.GetMilestonesAsync(principal, Fixture.ProjectId);
            Assert.Single(milestones);

            // Act
            await manager.DeleteAsync(principal, milestones.First());

            // Assert
            var milestonesAfterDelete = await manager.GetMilestonesAsync(principal, Fixture.ProjectId);
            Assert.Empty(milestonesAfterDelete);
        }

        private async Task DeleteAllProjectMilestonesAsync(ClaimsPrincipal principal)
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();

            foreach (var ms in await manager.GetMilestonesAsync(principal, Fixture.ProjectId))
            {
                await manager.DeleteAsync(principal, ms);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that a multiple milestones can be added
            """)]
        public async Task CreateMultipleMilestones_Success()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var principal = Fixture.App.SiteAdministrator;
            await DeleteAllProjectMilestonesAsync(principal);

            // Act
            int num = 10;
            for (int i = 0; i < num; i++)
            {
                await manager.AddMilestoneAsync(principal, new Milestone() { Title = "1.0." + num, TestProjectId = Fixture.ProjectId });
            }

            // Assert
            var milestones = await manager.GetMilestonesAsync(principal, Fixture.ProjectId);
            Assert.Equal(num, milestones.Count);
        }
    }
}
