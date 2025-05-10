using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;

namespace TestBucket.Domain.IntegrationTests.Milestones
{
    [IntegrationTest]
    [SecurityTest]
    [Feature("Milestones")]
    public class MilestonePermissionTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that a milestone cannot be deleted unless the user has delete permission
            """)]
        public async Task DeleteMilestone_WithoutPermission_UnauthorizedAccessExceptionThrown()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var principal = Fixture.App.SiteAdministrator;

            var user = Impersonation.Impersonate(builder => {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Write);
            });

            var ms = new Milestone() { Title = "1.0", TestProjectId = Fixture.ProjectId };
            await manager.AddMilestoneAsync(user, ms);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.DeleteAsync(user, ms);
            });
        }

        [Fact]
        [TestDescription("""
            Verifies that a milestone cannot be created unless the user has delete permission
            """)]
        public async Task AddMilestone_WithoutPermission_UnauthorizedAccessExceptionThrown()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var principal = Fixture.App.SiteAdministrator;

            var user = Impersonation.Impersonate(builder => {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Read);
            });

            var ms = new Milestone() { Title = "1.0", TestProjectId = Fixture.ProjectId };

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.AddMilestoneAsync(user, ms);
            });
        }


        [Fact]
        [TestDescription("""
            Verifies that a milestone cannot be updated by a user with correct permissions if the user
            belongs to another tenant
            """)]
        public async Task UpdateMilestone_WithWrongTenawnt_UnauthorizedAccessExceptionThrown()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IMilestoneManager>();
            var principal = Fixture.App.SiteAdministrator;

            var user1 = Impersonation.Impersonate(builder => {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Write);
            });
            var user2 = Impersonation.Impersonate(builder => {
                builder.TenantId = "other" + Fixture.App.Tenant;
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Read);
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Write);
            });

            var ms = new Milestone() { Title = "1.0", TestProjectId = Fixture.ProjectId };
            await manager.AddMilestoneAsync(user1, ms);
            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.UpdateMilestoneAsync(user2, ms);
            });
        }
    }
}
