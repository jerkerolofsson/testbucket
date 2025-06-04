using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Shared;
using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.UnitTests.Milestones
{
    /// <summary>
    /// Unit tests for <see cref="MilestoneManager"/>.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [SecurityTest]
    public class MilestonePermissionManagerTests
    {
        private readonly IMilestoneRepository _repository = Substitute.For<IMilestoneRepository>();
        private readonly MilestoneManager _manager;

        /// <summary>
        /// Creates a new milestone manager test instance
        /// </summary>
        public MilestonePermissionManagerTests()
        {
            _manager = new MilestoneManager(_repository);
        }


        /// <summary>
        /// Verifies that <see cref="MilestoneManager.AddMilestoneAsync"/> does not throws an exception when the user is authorized to add a milestone.
        /// </summary>
        [Fact]
        public async Task AddMilestoneAsync_WithWritePermission_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var milestone = new Milestone() { TenantId = "42" };
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.Add(PermissionEntityType.Issue, PermissionLevel.ReadWrite);
            });

            // Act
            await _manager.AddMilestoneAsync(principal, milestone);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.AddMilestoneAsync"/> throws an exception when the user is not authorized to add a milestone.
        /// </summary>
        [Theory]
        [InlineData(PermissionLevel.None)]
        [InlineData(PermissionLevel.Read)]
        [InlineData(PermissionLevel.Delete)]
        [InlineData(PermissionLevel.Approve)]
        public async Task AddMilestoneAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException(PermissionLevel level)
        {
            // Arrange
            var milestone = new Milestone() { TenantId = "42" };
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.Add(PermissionEntityType.Issue, level);
            });

            // Act/Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _manager.AddMilestoneAsync(principal, milestone);
            });
        }


        /// <summary>
        /// Verifies that <see cref="MilestoneManager.DeleteAsync"/> throws an exception when the user is not authorized to delete a milestone.
        /// </summary>
        [Theory]
        [InlineData(PermissionLevel.None)]
        [InlineData(PermissionLevel.Delete)]
        [InlineData(PermissionLevel.Write)]
        [InlineData(PermissionLevel.Approve)]
        public async Task GetMilestoneByNameAsync_WithoutReadPermission_ThrowsUnauthorizedAccessException(PermissionLevel level)
        {
            // Arrange
            var milestone = new Milestone() { TenantId = "42" };
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.Add(PermissionEntityType.Issue, level);
            });

            // Act/Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _manager.GetMilestoneByNameAsync(principal, 0, "milestone-name");
            });
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.DeleteAsync"/> throws an exception when the user is not authorized to delete a milestone.
        /// </summary>
        [Theory]
        [InlineData(PermissionLevel.None)]
        [InlineData(PermissionLevel.Read)]
        [InlineData(PermissionLevel.Approve)]
        [InlineData(PermissionLevel.Write)]
        public async Task DeleteAsync_WithoutDeletePermission_ThrowsUnauthorizedAccessException(PermissionLevel level)
        {
            // Arrange
            var milestone = new Milestone() { TenantId = "42" };
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.Add(PermissionEntityType.Issue, level);
            });

            // Act/Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _manager.DeleteAsync(principal, milestone);
            });
        }

    }
}