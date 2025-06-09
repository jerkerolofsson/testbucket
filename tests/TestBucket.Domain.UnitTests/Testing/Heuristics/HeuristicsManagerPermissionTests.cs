using NSubstitute;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.UnitTests.Testing.Heuristics
{
    /// <summary>
    /// Unit tests for <see cref="HeuristicsManager"/> permission enforcement.
    /// Verifies that operations throw <see cref="UnauthorizedAccessException"/> when the user lacks required permissions.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [SecurityTest]
    public class HeuristicsManagerPermissionTests
    {
        /// <summary>
        /// Mocked heuristics repository.
        /// </summary>
        private readonly IHeuristicsRepository _repo = Substitute.For<IHeuristicsRepository>();

        /// <summary>
        /// Mocked time provider.
        /// </summary>
        private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();

        /// <summary>
        /// The <see cref="HeuristicsManager"/> instance under test.
        /// </summary>
        private readonly HeuristicsManager _manager;

        /// <summary>
        /// Fixed current time used for testing.
        /// </summary>
        private readonly DateTimeOffset _now = new(2025, 6, 9, 12, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Initializes a new instance of the <see cref="HeuristicsManagerPermissionTests"/> class.
        /// Sets up the test environment with mocked dependencies.
        /// </summary>
        public HeuristicsManagerPermissionTests()
        {
            _timeProvider.GetUtcNow().Returns(_now);
            _manager = new HeuristicsManager(_timeProvider, _repo);
        }

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.AddAsync"/> throws <see cref="UnauthorizedAccessException"/>
        /// when the user does not have write permission.
        /// </summary>
        /// <param name="level">The permission level to test.</param>
        [InlineData(PermissionLevel.Read)]
        [InlineData(PermissionLevel.Delete)]
        [InlineData(PermissionLevel.Approve)]
        [InlineData(PermissionLevel.None)]
        [Theory]
        public async Task AddAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException(PermissionLevel level)
        {
            var heuristic = new Heuristic() { Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            var principal = Impersonation.Impersonate(user =>
            {
                user.UserName = "no-access-user";
                user.Add(PermissionEntityType.Heuristic, level);
                user.TenantId = "tenant1";
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _manager.AddAsync(principal, heuristic);
            });
        }

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.UpdateAsync"/> throws <see cref="UnauthorizedAccessException"/>
        /// when the user does not have write permission.
        /// </summary>
        /// <param name="level">The permission level to test.</param>
        [InlineData(PermissionLevel.Read)]
        [InlineData(PermissionLevel.Delete)]
        [InlineData(PermissionLevel.Approve)]
        [InlineData(PermissionLevel.None)]
        [Theory]
        public async Task UpdateAsync_WithoutWritePermission_ThrowsUnauthorizedAccessException(PermissionLevel level)
        {
            var heuristic = new Heuristic() { Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), TenantId = "tenant1" };
            var principal = Impersonation.Impersonate(user =>
            {
                user.UserName = "no-access-user";
                user.Add(PermissionEntityType.Heuristic, level);
                user.TenantId = "tenant1";
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _manager.UpdateAsync(principal, heuristic);
            });
        }

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.DeleteAsync"/> throws <see cref="UnauthorizedAccessException"/>
        /// when the user does not have delete permission.
        /// </summary>
        /// <param name="level">The permission level to test.</param>
        [InlineData(PermissionLevel.Read)]
        [InlineData(PermissionLevel.Write)]
        [InlineData(PermissionLevel.Approve)]
        [InlineData(PermissionLevel.None)]
        [Theory]
        public async Task DeleteAsync_WithoutDeletePermission_ThrowsUnauthorizedAccessException(PermissionLevel level)
        {
            var heuristic = new Heuristic() { Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), TenantId = "tenant1" };
            var principal = Impersonation.Impersonate(user =>
            {
                user.UserName = "no-access-user";
                user.Add(PermissionEntityType.Heuristic, level);
                user.TenantId = "tenant1";
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _manager.DeleteAsync(principal, heuristic);
            });
        }
    }
}