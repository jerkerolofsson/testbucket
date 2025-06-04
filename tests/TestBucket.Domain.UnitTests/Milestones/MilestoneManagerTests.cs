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
    [FunctionalTest]
    public class MilestoneManagerTests
    {
        private readonly IMilestoneRepository _repository = Substitute.For<IMilestoneRepository>();
        private readonly MilestoneManager _manager;

        /// <summary>
        /// Creates a new milestone manager test instance
        /// </summary>
        public MilestoneManagerTests()
        {
            _manager = new MilestoneManager(_repository);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.AddMilestoneAsync"/> sets audit fields and calls repository.
        /// </summary>
        [Fact]
        public async Task AddMilestoneAsync_SetsAuditFields_AndCallsRepository()
        {
            // Arrange
            var milestone = new Milestone();
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.AddAllPermissions();
            });

            // Act
            await _manager.AddMilestoneAsync(principal, milestone);

            // Assert
            Assert.Equal("42", milestone.TenantId);
            Assert.Equal("user", milestone.CreatedBy);
            await _repository.Received(1).AddMilestoneAsync(milestone);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.UpdateMilestoneAsync"/> sets audit fields and calls repository.
        /// </summary>
        [Fact]
        public async Task UpdateMilestoneAsync_SetsAuditFields_AndCallsRepository()
        {
            // Arrange
            var milestone = new Milestone() { TenantId = "42" };
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.AddAllPermissions();
            });
            // Act
            await _manager.UpdateMilestoneAsync(principal, milestone);

            // Assert
            Assert.Equal("user", milestone.ModifiedBy);
            await _repository.Received(1).UpdateMilestoneAsync(milestone);
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.GetMilestonesAsync"/> calls repository with correct filters.
        /// </summary>
        [Fact]
        public async Task GetMilestonesAsync_CallsRepositoryWithFilters()
        {
            // Arrange
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.AddAllPermissions();
            });
            // Act
            var result = await _manager.GetMilestonesAsync(principal, 1);

            // Assert
            await _repository.Received(1).GetMilestonesAsync(Arg.Any<IEnumerable<FilterSpecification<Milestone>>>());
        }

        /// <summary>
        /// Verifies that <see cref="MilestoneManager.DeleteAsync"/> calls repository with milestone ID.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_CallsRepositoryWithMilestoneId()
        {
            // Arrange
            var milestone = new Milestone { Id = 123, TenantId = "42" };
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.UserName = "user";
                builder.TenantId = "42";
                builder.AddAllPermissions();
            });
            // Act
            await _manager.DeleteAsync(principal, milestone);

            // Assert
            await _repository.Received(1).DeleteMilestoneByIdAsync(123);
        }
    }
}