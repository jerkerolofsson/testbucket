using NSubstitute;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.UnitTests.Testing.Heuristics
{
    /// <summary>
    /// Unit tests for <see cref="HeuristicsManager"/> covering add, update, delete, and search operations.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HeuristicsManagerTests"/> class.
        /// Sets up test dependencies and impersonates a user principal.
        /// </summary>
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

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.AddAsync"/> sets properties and calls the repository.
        /// </summary>
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

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.DeleteAsync"/> calls the repository with the correct ID.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_CallsRepository()
        {
            var heuristic = new Heuristic { Id = 42, TenantId = "tenant1", Description = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            _principal.SetTenantId("tenant1");
            await _manager.DeleteAsync(_principal, heuristic);

            await _repo.Received(1).DeleteAsync(42);
        }

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.UpdateAsync"/> sets the modified properties and calls the repository.
        /// </summary>
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

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.SearchAsync"/> applies the tenant filter and calls the repository.
        /// </summary>
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

    /// <summary>
    /// Helper extension for setting the tenant ID on a <see cref="ClaimsPrincipal"/> in tests.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Adds a tenant ID claim to the specified <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">The principal to modify.</param>
        /// <param name="tenantId">The tenant ID to set.</param>
        public static void SetTenantId(this ClaimsPrincipal principal, string tenantId)
        {
            var identity = (ClaimsIdentity)principal.Identity!;
            identity.AddClaim(new Claim("tenant_id", tenantId));
        }
    }
}