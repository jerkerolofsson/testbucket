using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.UnitTests.TestResources
{
    [EnrichedTest]
    [UnitTest]
    public class TestResourceDependencyAllocatorTests
    {
        [Component("TestResources")]
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

        [Component("TestResources")]
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

        [Component("TestResources")]
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


        [Component("TestResources")]
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

        [Component("TestResources")]
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

        internal TestResource? FirstOrDefault(IQueryable<TestResource> data, IEnumerable<FilterSpecification<TestResource>> filters)
        {
            foreach (var filter in filters)
            {
                data = data.Where(filter.Expression);
                if(data.Count() == 0)
                {

                }
            }
            return data.FirstOrDefault();
        }
    }
}
