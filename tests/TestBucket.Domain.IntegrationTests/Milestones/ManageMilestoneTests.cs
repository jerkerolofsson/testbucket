using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Milestones
{
    [IntegrationTest]
    [FunctionalTest]
    [Feature("Milestones")]
    [Component("Milestones")]
    public class ManageMilestoneTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that field options when the field trait is Milestone contains milestones added by a user
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetFieldOptions_ContainsMilestone()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var fieldDefinitionManager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
            var principal = Fixture.App.SiteAdministrator;
            await DeleteAllProjectMilestonesAsync(principal);

            var ms = new Milestone() { Title = "alpha", TestProjectId = Fixture.ProjectId };
            await manager.AddMilestoneAsync(principal, ms);

            // Act
            var fields = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneField = fields.Where(x=>x.TraitType == TraitType.Milestone).First();
            var options = await fieldDefinitionManager.GetOptionsAsync(principal, milestoneField);

            // Assert
            Assert.Single(options);
            var milestones = await manager.GetMilestonesAsync(principal, Fixture.ProjectId);
            Assert.Single(milestones);
            Assert.Equal(milestones[0].Title, options[0].Title);
        }

        /// <summary>
        /// Verifies that a milestone's description can be updated
        /// </summary>
        /// <returns></returns>
        [Fact]
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

        /// <summary>
        /// Verifies that a milestone can be created and deleted
        /// </summary>
        /// <returns></returns>
        [Fact]
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

        /// <summary>
        /// Verifies that a multiple milestones can be added
        /// </summary>
        /// <returns></returns>
        [Fact]
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
