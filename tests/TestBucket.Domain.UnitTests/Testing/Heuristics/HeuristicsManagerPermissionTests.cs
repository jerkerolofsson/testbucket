using NSubstitute;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.UnitTests.Testing.Heuristics
{
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [SecurityTest]
    public class HeuristicsManagerPermissionTests
    {
        private readonly IHeuristicsRepository _repo = Substitute.For<IHeuristicsRepository>();
        private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();
        private readonly HeuristicsManager _manager;
        private readonly DateTimeOffset _now = new(2025, 6, 9, 12, 0, 0, TimeSpan.Zero);

        public HeuristicsManagerPermissionTests()
        {
            _timeProvider.GetUtcNow().Returns(_now);
            _manager = new HeuristicsManager(_timeProvider, _repo);
        }

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