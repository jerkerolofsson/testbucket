using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

using NSubstitute;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Milestones
{
    /// <summary>
    /// Unit tests for <see cref="MilestoneManager"/>.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("Milestones")]
    public class MilestoneManagerTests
    {
        private static MilestoneManager CreateSut() => new MilestoneManager(new FakeMilestoneRepository());
        private const int ProjectId = 1;
        private const string TenantId = "tenant-1";

        private ClaimsPrincipal CreateUserWithLimitedPermission(PermissionLevel level)
        {
            return Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = TenantId;
                builder.Add(PermissionEntityType.Issue, level);
            });
        }

        private ClaimsPrincipal CreateUserWithAllPermissions()
        {
            return Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = TenantId;
                builder.AddAllPermissions();
            });
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.AddMilestoneAsync"/> sets audit fields and calls repository.
        /// </summary>
        [FunctionalTest]
        [Fact]
        public async Task AddMilestoneAsync_SetsAuditFields_AndCallsRepository()
        {
            // Arrange
            var manager = CreateSut();
            var milestone = new Milestone() { TestProjectId = ProjectId };
            var principal = CreateUserWithAllPermissions();

            // Act
            await manager.AddMilestoneAsync(principal, milestone);

            // Assert
            Assert.Equal(TenantId, milestone.TenantId);
            Assert.Equal("user", milestone.CreatedBy);
            var storedMilestones = await manager.GetMilestonesAsync(principal, ProjectId);
            Assert.Single(storedMilestones);
            var storedMilestone = storedMilestones.First();
            Assert.Equal(TenantId, storedMilestone.TenantId);
            Assert.Equal("user", storedMilestone.CreatedBy);

        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.UpdateMilestoneAsync"/> sets audit fields and calls repository.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task UpdateMilestoneAsync_SetsAuditFields_AndCallsRepository()
        {
            // Arrange
            var manager = CreateSut();
            var milestone = new Milestone() { TenantId = TenantId, TestProjectId = ProjectId, Title = "123" };
            var user1 = CreateUserWithAllPermissions();

            await manager.AddMilestoneAsync(user1, milestone);

            // Act
            var user2 = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user2";
                builder.TenantId = TenantId;
                builder.AddAllPermissions();
            });
            var updatedMilestone = new Milestone() { TenantId = TenantId, TestProjectId = ProjectId, Id = milestone.Id, Title = "abc" };
            await manager.UpdateMilestoneAsync(user2, updatedMilestone);

            // Assert
            var storedMilestones = await manager.GetMilestonesAsync(user1, ProjectId);
            Assert.Single(storedMilestones);
            var storedMilestone = storedMilestones.First();
            Assert.Equal("abc", storedMilestone.Title);
            Assert.Equal("user2", storedMilestone.ModifiedBy);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.GetMilestonesAsync"/> calls repository with correct filters.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task GetMilestonesAsync_WithProjectIdFilter_CorrectResponse()
        {
            // Arrange
            var manager = CreateSut();
            var principal = CreateUserWithAllPermissions();

            await manager.AddMilestoneAsync(principal, new Milestone() { TenantId = TenantId, TestProjectId = ProjectId, Title = "123" });
            await manager.AddMilestoneAsync(principal, new Milestone() { TenantId = TenantId, TestProjectId = ProjectId+1, Title = "234" });


            // Act
            var result = await manager.GetMilestonesAsync(principal, ProjectId);

            // Assert
            Assert.Single(result);
            var storedMilestone = result.First();
            Assert.Equal("123", storedMilestone.Title);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.DeleteAsync"/> calls repository with milestone ID.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task DeleteAsync_DeletesCorrectMilestone()
        {
            // Arrange
            var manager = CreateSut();
            var milestone1 = new Milestone { TenantId = TenantId, Title="1", TestProjectId = ProjectId };
            var milestone2 = new Milestone { TenantId = TenantId, Title="2", TestProjectId = ProjectId };
            var principal = CreateUserWithAllPermissions();

            await manager.AddMilestoneAsync(principal, milestone1);
            await manager.AddMilestoneAsync(principal, milestone2);

            // Act
            await manager.DeleteAsync(principal, milestone1);

            // Assert
            var result = await manager.GetMilestonesAsync(principal, ProjectId);
            Assert.Single(result);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.SearchMilestonesAsync"/> returns milestones matching the search text, offset, and count.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task SearchMilestonesAsync_ReturnsMatchingMilestones_WithOffsetAndCount()
        {
            // Arrange
            var milestones = new List<Milestone>
            {
                new Milestone { Id = 1, Title = "Alpha", TenantId = TenantId, TestProjectId = ProjectId },
                new Milestone { Id = 2, Title = "Beta", TenantId = TenantId, TestProjectId = ProjectId },
                new Milestone { Id = 3, Title = "Gamma", TenantId = TenantId, TestProjectId = ProjectId },
                new Milestone { Id = 4, Title = "AlphaX", TenantId = TenantId, TestProjectId = ProjectId }
            };
            var manager = CreateSut();
            var principal = CreateUserWithAllPermissions();

            foreach (var ms in milestones)
            {
                await manager.AddMilestoneAsync(principal, ms);
            }


            // Act
            var result = await manager.SearchMilestonesAsync(principal, ProjectId, "Alpha", 0, 2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Contains("Alpha", m.Title));
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.SearchMilestonesAsync"/> applies offset and count correctly.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task SearchMilestonesAsync_AppliesOffsetAndCount()
        {
            // Arrange
            var milestones = new List<Milestone>
            {
                new Milestone { Id = 1, Title = "Alpha", TenantId = TenantId, TestProjectId = ProjectId },
                new Milestone { Id = 2, Title = "AlphaX", TenantId = TenantId, TestProjectId = ProjectId },
                new Milestone { Id = 3, Title = "AlphaY", TenantId = TenantId, TestProjectId = ProjectId }
            };
            var manager = CreateSut();
            var principal = CreateUserWithAllPermissions();

            foreach (var ms in milestones)
            {
                await manager.AddMilestoneAsync(principal, ms);
            }
            // Act
            var result = await manager.SearchMilestonesAsync(principal, ProjectId, "Alpha", 1, 1);
            
            // Assert
            Assert.Single(result);
            Assert.Equal("AlphaX", result[0].Title);
        }

        /// <summary>
        /// Verifies that milestones are not cross-contaminated between tenants.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task GetMilestonesAsync_DifferentTenants_NoCrossContamination()
        {
            // Arrange
            var manager = CreateSut();
            var principalTenant1 = CreateUserWithAllPermissions();

            var principalTenant2 = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user2";
                builder.TenantId = "tenant2";
                builder.AddAllPermissions();
            });
            await manager.AddMilestoneAsync(principalTenant1, new Milestone { TestProjectId = ProjectId, Title = "T1-M1" });
            await manager.AddMilestoneAsync(principalTenant2, new Milestone { TestProjectId = ProjectId, Title = "T2-M1" });

            // Act
            var milestonesTenant1 = await manager.GetMilestonesAsync(principalTenant1, ProjectId);
            var milestonesTenant2 = await manager.GetMilestonesAsync(principalTenant2, ProjectId);

            // Assert
            Assert.Single(milestonesTenant1);
            Assert.Equal(TenantId, milestonesTenant1.First().TenantId);
            Assert.Equal("T1-M1", milestonesTenant1.First().Title);
            Assert.Single(milestonesTenant2);
            Assert.Equal("tenant2", milestonesTenant2.First().TenantId);
            Assert.Equal("T2-M1", milestonesTenant2.First().Title);
        }

        /// <summary>
        /// Verifies that UnauthorizedAccessException is thrown when deleting a milestone with a user from another tenant.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task DeleteMilestoneAsync_DifferentTenant_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var manager = CreateSut();
            var milestone = new Milestone { TenantId = TenantId, TestProjectId = ProjectId, Title = "T1-M1" };
            var principalTenant1 = CreateUserWithAllPermissions();

            await manager.AddMilestoneAsync(principalTenant1, milestone);

            var principalTenant2 = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user2";
                builder.TenantId = "tenant2";
                builder.AddAllPermissions();
            });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.DeleteAsync(principalTenant2, milestone));
        }
        /// <summary>
        /// Verifies that UnauthorizedAccessException is thrown when updating a milestone with a user from another tenant.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task UpdateMilestoneAsync_DifferentTenant_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var manager = CreateSut();
            var milestone = new Milestone { TenantId = TenantId, TestProjectId = ProjectId, Title = "T1-M1" };
            var principalTenant1 = CreateUserWithAllPermissions();

            await manager.AddMilestoneAsync(principalTenant1, milestone);

            var principalTenant2 = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user2";
                builder.TenantId = "tenant2";
                builder.AddAllPermissions();
            });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.UpdateMilestoneAsync(principalTenant2, milestone));
        }

        /// <summary>
        /// Verifies that AddMilestoneAsync enforces access levels.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task AddMilestoneAsync_InsufficientPermission_ThrowsUnauthorizedAccessException()
        {
            var manager = CreateSut();
            var milestone = new Milestone() { TestProjectId = ProjectId };
            var principal = CreateUserWithLimitedPermission(PermissionLevel.Read); // No Write
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.AddMilestoneAsync(principal, milestone));
        }

        /// <summary>
        /// Verifies that UpdateMilestoneAsync enforces access levels.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task UpdateMilestoneAsync_InsufficientPermission_ThrowsUnauthorizedAccessException()
        {
            var manager = CreateSut();
            var milestone = new Milestone() { TenantId = TenantId, TestProjectId = ProjectId, Title = "123" };
            var principal = CreateUserWithAllPermissions();
            await manager.AddMilestoneAsync(principal, milestone);
            var limitedPrincipal = CreateUserWithLimitedPermission(PermissionLevel.Read); // No Write
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.UpdateMilestoneAsync(limitedPrincipal, milestone));
        }

        /// <summary>
        /// Verifies that GetMilestonesAsync enforces access levels.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task GetMilestonesAsync_InsufficientPermission_ThrowsUnauthorizedAccessException()
        {
            var manager = CreateSut();
            var principal = CreateUserWithLimitedPermission(PermissionLevel.None); // No Read
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.GetMilestonesAsync(principal, ProjectId));
        }

        /// <summary>
        /// Verifies that GetMilestoneByNameAsync enforces access levels.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task GetMilestoneByNameAsync_InsufficientPermission_ThrowsUnauthorizedAccessException()
        {
            var manager = CreateSut();
            var principal = CreateUserWithLimitedPermission(PermissionLevel.None); // No Read
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.GetMilestoneByNameAsync(principal, ProjectId, "Test"));
        }

        /// <summary>
        /// Verifies that SearchMilestonesAsync enforces access levels.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task SearchMilestonesAsync_InsufficientPermission_ThrowsUnauthorizedAccessException()
        {
            var manager = CreateSut();
            var principal = CreateUserWithLimitedPermission(PermissionLevel.None); // No Read
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.SearchMilestonesAsync(principal, ProjectId, "Test", 0, 10));
        }

        /// <summary>
        /// Verifies that DeleteAsync enforces access levels.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task DeleteAsync_InsufficientPermission_ThrowsUnauthorizedAccessException()
        {
            var manager = CreateSut();
            var milestone = new Milestone { TenantId = TenantId, Title = "1", TestProjectId = ProjectId };
            var principal = CreateUserWithAllPermissions();
            await manager.AddMilestoneAsync(principal, milestone);
            var limitedPrincipal = CreateUserWithLimitedPermission(PermissionLevel.Write); // No Delete
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await manager.DeleteAsync(limitedPrincipal, milestone));
        }
    }
}