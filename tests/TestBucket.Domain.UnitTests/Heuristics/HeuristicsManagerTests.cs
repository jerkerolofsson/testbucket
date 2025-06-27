using NSubstitute;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TestBucket.Contracts;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;
using Xunit;

namespace TestBucket.Domain.UnitTests.Heuristics
{
    /// <summary>
    /// Unit tests for <see cref="HeuristicsManager"/> verifying timestamp behavior using a fake time provider.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("Heuristics")]
    public class HeuristicsManagerTests
    {
        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.AddAsync"/> sets both Created and Modified to the injected time.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task AddAsync_SetsCreatedAndModifiedToInjectedTime()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);

            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            var heuristic = new Heuristic { Name = "H", Description = "desc" };

            // Act
            await manager.AddAsync(principal, heuristic);

            // Assert
            Assert.Equal(fakeNow, heuristic.Created);
            Assert.Equal(fakeNow, heuristic.Modified);
            Assert.Equal("user1", heuristic.CreatedBy);
            Assert.Equal("user1", heuristic.ModifiedBy);
            await repo.Received(1).AddAsync(heuristic);
        }

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.UpdateAsync"/> changes Modified and ModifiedBy but not Created or CreatedBy.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task UpdateAsync_SetsCreatedAndModifiedToInjectedTime()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeUpdateNow = new DateTimeOffset(2026, 2, 17, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);

            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principalAdd = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            var heuristic = new Heuristic { Name = "H", Description = "desc" };
            await manager.AddAsync(principalAdd, heuristic);

            // Act
            fakeTimeProvider.SetDateTime(fakeUpdateNow);
            var principalUpdate = TestPrincipal.WithTenantAndName("tenant-1", "user2");
            await manager.UpdateAsync(principalUpdate, heuristic);

            // Assert
            Assert.Equal(fakeNow, heuristic.Created);
            Assert.Equal(fakeUpdateNow, heuristic.Modified);
            Assert.Equal("user1", heuristic.CreatedBy);
            Assert.Equal("user2", heuristic.ModifiedBy);
            await repo.Received(1).AddAsync(heuristic);
        }


        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.UpdateAsync"/> sets Modified to the injected time.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task UpdateAsync_SetsModifiedToInjectedTime()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);

            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-2", "user2");
            var heuristic = new Heuristic { Name = "H2", Description = "desc2", TenantId = "tenant-2" };

            // Act
            await manager.UpdateAsync(principal, heuristic);

            // Assert
            Assert.Equal(fakeNow, heuristic.Modified);
            Assert.Equal("user2", heuristic.ModifiedBy);
            await repo.Received(1).UpdateAsync(heuristic);
        }

        /// <summary>
        /// Verifies that <see cref="HeuristicsManager.DeleteAsync"/> calls the repository to delete the heuristic.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task DeleteAsync_DeletesHeuristicById()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);

            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            var heuristic = new Heuristic { Id = 123, Name = "H", Description = "desc", TenantId = "tenant-1" };

            // Act
            await manager.DeleteAsync(principal, heuristic);

            // Assert
            await repo.Received(1).DeleteAsync(heuristic.Id);
        }

        /// <summary>
        /// Verifies that SearchAsync always includes a tenant filter, even if no filters are provided.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task SearchAsync_AlwaysIncludesTenantFilter_WhenNoFiltersProvided()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);
            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            FilterSpecification<Heuristic>[]? capturedFilters = null;

            repo.SearchAsync(Arg.Do<FilterSpecification<Heuristic>[]>(f => capturedFilters = f), 0, 10)
                .Returns(new PagedResult<Heuristic>() { Items = [], TotalCount = 0 });

            // Act
            await manager.SearchAsync(principal, [], 0, 10);

            // Assert
            Assert.NotNull(capturedFilters);
            Assert.Contains(capturedFilters, f => f.GetType().Name.Contains("FilterByTenant"));
        }

        /// <summary>
        /// Verifies that SearchAsync always includes a tenant filter, even if other filters are provided.
        /// </summary>
        [Fact]
        [FunctionalTest]
        public async Task SearchAsync_AlwaysIncludesTenantFilter_WhenOtherFiltersProvided()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);
            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            FilterSpecification<Heuristic>[]? capturedFilters = null;

            var customFilter = Substitute.For<FilterSpecification<Heuristic>>();
            repo.SearchAsync(Arg.Do<FilterSpecification<Heuristic>[]>(f => capturedFilters = f), 0, 10)
                .Returns(new PagedResult<Heuristic>() { Items = [], TotalCount = 0 });

            // Act
            await manager.SearchAsync(principal, new[] { customFilter }, 0, 10);

            // Assert
            Assert.NotNull(capturedFilters);
            Assert.Contains(capturedFilters, f => f.GetType().Name.Contains("FilterByTenant"));
            Assert.Contains(customFilter, capturedFilters);
        }

        /// <summary>
        /// Verifies that UpdateAsync throws if the principal's tenant does not match the heuristic's tenant.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task UpdateAsync_ThrowsIfTenantIsDifferent()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);

            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            var heuristic = new Heuristic { Name = "H", Description = "desc", TenantId = "tenant-2" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                manager.UpdateAsync(principal, heuristic));
        }

        /// <summary>
        /// Verifies that DeleteAsync throws if the principal's tenant does not match the heuristic's tenant.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task DeleteAsync_ThrowsIfTenantIsDifferent()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);

            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = TestPrincipal.WithTenantAndName("tenant-1", "user1");
            var heuristic = new Heuristic { Id = 123, Name = "H", Description = "desc", TenantId = "tenant-2" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                manager.DeleteAsync(principal, heuristic));
        }

        /// <summary>
        /// Verifies that AddAsync throws if the principal lacks write permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task AddAsync_ThrowsIfNoWritePermission()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);
            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            // Principal with no Heuristic permissions
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "tenant-1";
                builder.UserName = "user1";
                builder.Email = "user1";
                // No permissions for Heuristic
            });

            var heuristic = new Heuristic { Name = "H", Description = "desc" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                manager.AddAsync(principal, heuristic));
        }

        /// <summary>
        /// Verifies that UpdateAsync throws if the principal lacks write permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task UpdateAsync_ThrowsIfNoWritePermission()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);
            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "tenant-1";
                builder.UserName = "user1";
                builder.Email = "user1";
                // No permissions for Heuristic
            });

            var heuristic = new Heuristic { Name = "H", Description = "desc", TenantId = "tenant-1" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                manager.UpdateAsync(principal, heuristic));
        }

        /// <summary>
        /// Verifies that DeleteAsync throws if the principal lacks delete permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task DeleteAsync_ThrowsIfNoDeletePermission()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);
            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "tenant-1";
                builder.UserName = "user1";
                builder.Email = "user1";
                // No permissions for Heuristic
            });

            var heuristic = new Heuristic { Id = 123, Name = "H", Description = "desc", TenantId = "tenant-1" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                manager.DeleteAsync(principal, heuristic));
        }

        /// <summary>
        /// Verifies that SearchAsync throws if the principal lacks read permission.
        /// </summary>
        [Fact]
        [SecurityTest]
        public async Task SearchAsync_ThrowsIfNoReadPermission()
        {
            // Arrange
            var fakeNow = new DateTimeOffset(2025, 6, 27, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeNow);
            var repo = Substitute.For<IHeuristicsRepository>();
            var manager = new HeuristicsManager(fakeTimeProvider, repo);

            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "tenant-1";
                builder.UserName = "user1";
                builder.Email = "user1";
                // No permissions for Heuristic
            });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                manager.SearchAsync(principal, [], 0, 10));
        }
    }


    /// <summary>
    /// Helper for creating test principals with tenant and name.
    /// </summary>
    internal static class TestPrincipal
    {
        public static ClaimsPrincipal WithTenantAndName(string tenantId, string name)
        {
            return Impersonation.Impersonate(configure =>
            {
                configure.TenantId = tenantId;
                configure.UserName = name;
                configure.Email = name;
                configure.Add(Domain.Identity.Permissions.PermissionEntityType.Heuristic, Domain.Identity.Permissions.PermissionLevel.All);
            });
        }
    }
}