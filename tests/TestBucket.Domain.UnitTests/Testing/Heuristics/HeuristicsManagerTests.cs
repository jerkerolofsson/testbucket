using NSubstitute;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.UnitTests.Testing.Heuristics
{
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [FunctionalTest]
    public class HeuristicsManagerTests
    {
        private readonly IHeuristicsRepository _repo = Substitute.For<IHeuristicsRepository>();
        private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();
        private readonly HeuristicsManager _manager;
        private readonly ClaimsPrincipal _principal;
        private readonly DateTimeOffset _now = new(2025, 6, 9, 12, 0, 0, TimeSpan.Zero);

        public HeuristicsManagerTests()
        {
            _timeProvider.GetUtcNow().Returns(_now);
            _manager = new HeuristicsManager(_timeProvider, _repo);

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") });
            _principal = new ClaimsPrincipal(identity);

            _principal = Impersonation.Impersonate(user =>
            {
                user.UserName = "user";
                user.TenantId = "tenant1";
                user.AddAllPermissions();
            });
        }

        [Fact]
        public async Task AddAsync_SetsPropertiesAndCallsRepository()
        {
            var heuristic = new Heuristic() { Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            _principal.SetTenantId("tenant1");
            await _manager.AddAsync(_principal, heuristic);

            Assert.Equal("tenant1", heuristic.TenantId);
            Assert.Equal("user", heuristic.CreatedBy);
            Assert.Equal("user", heuristic.ModifiedBy);
            Assert.Equal(_now, heuristic.Created);
            Assert.Equal(_now, heuristic.Modified);
            await _repo.Received(1).AddAsync(heuristic);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepository()
        {
            var heuristic = new Heuristic { Id = 42, TenantId = "tenant1", Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            _principal.SetTenantId("tenant1");
            await _manager.DeleteAsync(_principal, heuristic);

            await _repo.Received(1).DeleteAsync(42);
        }

        [Fact]
        public async Task UpdateAsync_SetsModifiedAndCallsRepository()
        {
            var heuristic = new Heuristic { TenantId = "tenant1", Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            _principal.SetTenantId("tenant1");
            await _manager.UpdateAsync(_principal, heuristic);

            Assert.Equal("user", heuristic.ModifiedBy);
            Assert.Equal(_now, heuristic.Modified);
            await _repo.Received(1).UpdateAsync(heuristic);
        }

        [Fact]
        public async Task SearchAsync_CallsRepositoryWithTenantFilter()
        {
            var paged = new PagedResult<Heuristic>() { Items = [], TotalCount = 0 };
            _principal.SetTenantId("tenant1");
            _repo.SearchAsync(Arg.Any<FilterSpecification<Heuristic>[]>(), 0, 10).Returns(paged);

            var result = await _manager.SearchAsync(_principal, Array.Empty<FilterSpecification<Heuristic>>(), 0, 10);

            await _repo.Received(1).SearchAsync(
                Arg.Is<FilterSpecification<Heuristic>[]>(filters =>
                    filters.Any(f => f is FilterByTenant<Heuristic>)), 0, 10);
        }
    }

    // Helper extension for setting tenant id in tests
    public static class ClaimsPrincipalExtensions
    {
        public static void SetTenantId(this ClaimsPrincipal principal, string tenantId)
        {
            var identity = (ClaimsIdentity)principal.Identity!;
            identity.AddClaim(new Claim("tenant_id", tenantId));
        }
    }
}