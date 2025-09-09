using System.Security.Claims;

using Mediator;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using NSubstitute;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.UnitTests.TestResources.Fakes;

namespace TestBucket.Domain.UnitTests.TestResources
{
    /// <summary>
    /// Contains unit tests for <see cref="TestResourceDependencyAllocator"/> allocation filters.
    /// Tests ensure that only enabled, healthy, unlocked, and tenant-matching resources of the correct type are selected.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [FunctionalTest]
    [Component("Test Resources")]
    public class TestResourceDependencyAllocatorTests
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResourceDependencyAllocatorTests"/> class.
        /// </summary>
        public TestResourceDependencyAllocatorTests()
        {
            _mediator = Substitute.For<IMediator>();
        }

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission level.
        /// </summary>
        /// <param name="level">The permission level to assign to the principal.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
        private ClaimsPrincipal CreatePrincipal(PermissionLevel level)
        {
            return Impersonation.Impersonate(builder =>
            {
                builder.TenantId = "tenant-1";
                builder.UserName = "user1@testbucket.io";
                builder.Add(PermissionEntityType.TestResource, level);
            });
        }

        private TestResourceManager CreateTestResourceManager()
        {
            return new TestResourceManager(new FakeTestResourceRepository(), TimeProvider.System, _mediator);
        }

        private TestResourceDependencyAllocator CreateSut() => new TestResourceDependencyAllocator(CreateTestResourceManager(), _mediator);
        private async Task<TestResourceDependencyAllocator> CreateSutAsync(Func<TestResourceManager, Task> configure)
        {
            var manager = CreateTestResourceManager();
            await configure(manager);
            return new TestResourceDependencyAllocator(manager, _mediator);
        }

        /// <summary>
        /// Verifies that disabled resources are excluded by the allocation filters and only enabled resources are matched.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithDisabledResources_ReturnsCorrectMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = [
                new TestResource() { Name = "a", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Healthy, Enabled = false},
                new TestResource() { Name = "b", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Healthy, Enabled = false},
                new TestResource() { Name = "c", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                ];

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.NotNull(match);
            Assert.Equal("c", match.Name);
        }

        /// <summary>
        /// Verifies that unhealthy resources are excluded by the allocation filters and only healthy resources are matched.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithUnhealthyResources_ReturnsCorrectMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = [
                new TestResource() { Name = "a", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Unhealthy, Enabled = true},
                new TestResource() { Name = "b", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Unhealthy, Enabled = true},
                new TestResource() { Name = "c", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                ];

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.NotNull(match);
            Assert.Equal("c", match.Name);
        }

        /// <summary>
        /// Verifies that locked resources are excluded by the allocation filters and only unlocked resources are matched.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithLockedResource_ReturnsCorrectMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = [
                new TestResource() { Name = "a", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Locked = true, Health = HealthStatus.Healthy, Enabled = true},
                new TestResource() { Name = "b", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Locked = false, Health = HealthStatus.Healthy, Enabled = true},
                new TestResource() { Name = "c", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Locked = true, Health = HealthStatus.Healthy, Enabled = true},
                ];

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.NotNull(match);
            Assert.Equal("b", match.Name);
        }

        /// <summary>
        /// Verifies that only resources with a matching type are selected by the allocation filters.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithMatchingResourceType_ReturnsCorrectMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = [
                new TestResource() { Name = "a", TenantId = "1234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                new TestResource() { Name = "b", TenantId = "1234", ResourceId = "1", Types = ["android", "tablet"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                new TestResource() { Name = "c", TenantId = "1234", ResourceId = "1", Types = ["com_port"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                ];

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.NotNull(match);
            Assert.Equal("a", match.Name);
        }

        /// <summary>
        /// Verifies that resources with a non-matching tenant are excluded by the allocation filters.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithoutMatchingTenant_ReturnsNoMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = [
                new TestResource() { Name = "a", TenantId = "2234", ResourceId = "1", Types = ["android", "phone"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                new TestResource() { Name = "b", TenantId = "2234", ResourceId = "1", Types = ["android", "tablet"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                new TestResource() { Name = "c", TenantId = "2234", ResourceId = "1", Types = ["com_port"], Owner = "", Health = HealthStatus.Healthy, Enabled = true},
                ];

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.Null(match);
        }

        /// <summary>
        /// Applies a sequence of <see cref="FilterSpecification{TestResource}"/> filters to a queryable collection and returns the first matching resource, or <c>null</c> if none match.
        /// </summary>
        /// <param name="data">The queryable collection of <see cref="TestResource"/> objects.</param>
        /// <param name="filters">The allocation filters to apply.</param>
        /// <returns>The first matching <see cref="TestResource"/>, or <c>null</c> if no match is found.</returns>
        internal TestResource? FirstOrDefault(IQueryable<TestResource> data, IEnumerable<FilterSpecification<TestResource>> filters)
        {
            foreach (var filter in filters)
            {
                data = data.Where(filter.Expression);
            }
            return data.FirstOrDefault();
        }

        /// <summary>
        /// Verifies that no matches are returned when the resource list is empty.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithEmptyResourceList_ReturnsNoMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = new List<TestResource>();

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.Null(match);
        }

        /// <summary>
        /// Verifies that the correct resource is selected when multiple resources match the filters.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithMultipleMatchingResources_ReturnsFirstMatch()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = new List<TestResource>
            {
                new TestResource() { Name = "a", TenantId = "1234", ResourceId = "1", Types = new[] { "android", "phone" }, Owner = "", Health = HealthStatus.Healthy, Enabled = true },
                new TestResource() { Name = "b", TenantId = "1234", ResourceId = "2", Types = new[] { "android", "phone" }, Owner = "", Health = HealthStatus.Healthy, Enabled = true },
            };

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.NotNull(match);
            Assert.Equal("a", match.Name);
        }


        /// <summary>
        /// Verifies behavior when the ClaimsPrincipal is null.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithNullPrincipal_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TestResourceDependencyAllocator.GetAllocationFilters(null!, "phone"));
        }

        /// <summary>
        /// Verifies behavior when the resource type is null.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithNullResourceType_ThrowsArgumentNullException()
        {
            var principal = Impersonation.Impersonate("1234");

            Assert.Throws<ArgumentNullException>(() => TestResourceDependencyAllocator.GetAllocationFilters(principal, null!));
        }

        /// <summary>
        /// Verifies that resources are filtered correctly when multiple filters interact.
        /// </summary>
        [Fact]
        public void GetAllocationFilters_WithMultipleFilters_ReturnsCorrectMatches()
        {
            var principal = Impersonation.Impersonate("1234");

            var filters = TestResourceDependencyAllocator.GetAllocationFilters(principal, "phone");

            List<TestResource> data = new List<TestResource>
            {
                new TestResource() { Name = "a", TenantId = "1234", ResourceId = "1", Types = new[] { "android", "phone" }, Owner = "", Health = HealthStatus.Unhealthy, Enabled = true },
                new TestResource() { Name = "b", TenantId = "1234", ResourceId = "2", Types = new[] { "android", "phone" }, Owner = "", Health = HealthStatus.Healthy, Enabled = false },
                new TestResource() { Name = "c", TenantId = "1234", ResourceId = "3", Types = new[] { "android", "phone" }, Owner = "", Health = HealthStatus.Healthy, Enabled = true },
            };

            var match = FirstOrDefault(data.AsQueryable(), filters);
            Assert.NotNull(match);
            Assert.Equal("c", match.Name);
        }

        /// <summary>
        /// Verifies that CollectDependenciesAsync returns a TestResourceBag with the correct resources when valid inputs are provided.
        /// </summary>
        [Fact]
        public async Task CollectDependenciesAsync_WithValidInputs_ReturnsCorrectResourceBag()
        {
            // Arrange
            var principal = CreatePrincipal(PermissionLevel.All);
            var sut = await CreateSutAsync(async (manager) =>
            {
                await manager.AddAsync(principal, new TestResource { Name = "phone1", Types = ["phone"], Owner = "owner-1", ResourceId = "id1", Enabled = true, Health = HealthStatus.Healthy });
                await manager.AddAsync(principal, new TestResource { Name = "tablet1", Types = ["tablet"], Owner = "owner-1", ResourceId = "id2", Enabled = true, Health = HealthStatus.Healthy });
            });

            var context = new TestExecutionContext
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = 1,
                ProjectId = 1,
                TeamId = 1,
                Dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" },
                    new TestCaseDependency { ResourceType = "tablet" }
                }
            };

            // Act
            var result = await sut.CollectDependenciesAsync(principal, context, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestResourceBag>(result);
            Assert.Equal(2, result.Resources.Count);
        }

        /// <summary>
        /// Verifies that CollectDependenciesAsync allocates two unique resources when two resources of the same type are requested.
        /// </summary>
        [Fact]
        public async Task CollectDependenciesAsync_WithTwoOfSameType_TwoUniqueAllocated()
        {
            // Arrange
            var principal = CreatePrincipal(PermissionLevel.All);
            var sut = await CreateSutAsync(async (manager) =>
            {
                await manager.AddAsync(principal, new TestResource { Name = "phone1", Types = ["phone"], Owner = "owner-1", ResourceId = "id1", Enabled = true, Health = HealthStatus.Healthy });
                await manager.AddAsync(principal, new TestResource { Name = "phone2", Types = ["phone"], Owner = "owner-1", ResourceId = "id2", Enabled = true, Health = HealthStatus.Healthy });
            });

            var context = new TestExecutionContext
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = 1,
                ProjectId = 1,
                TeamId = 1,
                Dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" },
                    new TestCaseDependency { ResourceType = "phone" }
                }
            };

            // Act
            var result = await sut.CollectDependenciesAsync(principal, context, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Resources.Count);
            Assert.True(result.Resources[0].Name != result.Resources[1].Name);
        }

        /// <summary>
        /// Verifies that CollectDependenciesAsync returns an empty TestResourceBag with correct resources when valid inputs are provided but all the resoures are disabled.
        /// </summary>
        [Fact]
        public async Task CollectDependenciesAsync_WithValidInputs_DisabledResources_AllocationFails()
        {
            // Arrange
            var principal = CreatePrincipal(PermissionLevel.All);
            var sut = await CreateSutAsync(async (manager) =>
            {
                await manager.AddAsync(principal, new TestResource { Name = "phone1", Types = ["phone"], Owner = "owner-1", ResourceId = "id1", Enabled = false, Health = HealthStatus.Healthy });
                await manager.AddAsync(principal, new TestResource { Name = "tablet1", Types = ["tablet"], Owner = "owner-1", ResourceId = "id2", Enabled = false, Health = HealthStatus.Healthy });
            });

            var context = new TestExecutionContext
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = 1,
                ProjectId = 1,
                TeamId = 1,
                Dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" },
                    new TestCaseDependency { ResourceType = "tablet" }
                }
            };

            // Act
            var result = await sut.CollectDependenciesAsync(principal, context, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Resources);
        }

        /// <summary>
        /// Verifies that CollectDependenciesAsync returns an empty TestResourceBag with correct resources when valid inputs are provided but all the resoures are unhealrhy.
        /// </summary>
        [Fact]
        public async Task CollectDependenciesAsync_WithValidInputs_UnhealthyResources_AllocationFails()
        {
            // Arrange
            var principal = CreatePrincipal(PermissionLevel.All);
            var sut = await CreateSutAsync(async (manager) =>
            {
                await manager.AddAsync(principal, new TestResource { Name = "phone1", Types = ["phone"], Owner = "owner-1", ResourceId = "id1", Enabled = true, Health = HealthStatus.Unhealthy });
                await manager.AddAsync(principal, new TestResource { Name = "tablet1", Types = ["tablet"], Owner = "owner-1", ResourceId = "id2", Enabled = true, Health = HealthStatus.Unhealthy });
            });

            var context = new TestExecutionContext
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = 1,
                ProjectId = 1,
                TeamId = 1,
                Dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" },
                    new TestCaseDependency { ResourceType = "tablet" }
                }
            };

            // Act
            var result = await sut.CollectDependenciesAsync(principal, context, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Resources);
        }

        /// <summary>
        /// Verifies that CollectDependenciesAsync returns an empty TestResourceBag when no dependencies are specified.
        /// </summary>
        [Fact]
        public async Task CollectDependenciesAsync_WithNoDependencies_ReturnsEmptyResourceBag()
        {
            // Arrange
            var sut = CreateSut();
            var principal = CreatePrincipal(PermissionLevel.Read);
            var context = new TestExecutionContext
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = 1,
                ProjectId = 1,
                TeamId = 1,
                Dependencies = new List<TestCaseDependency>()
            };

            // Act
            var result = await sut.CollectDependenciesAsync(principal, context, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestResourceBag>(result);
            Assert.Empty(result.Resources);
        }
    }
}