using Microsoft.Extensions.Diagnostics.HealthChecks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;

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
                if (data.Count() == 0)
                {

                }
            }
            return data.FirstOrDefault();
        }
    }
}