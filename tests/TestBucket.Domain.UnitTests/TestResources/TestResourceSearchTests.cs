using Microsoft.Extensions.Diagnostics.HealthChecks;
using TestBucket.Domain.TestResources.Models;
using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.UnitTests.TestResources
{
    /// <summary>
    /// Contains unit tests for searching and filtering <see cref="TestResource"/> instances.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [FunctionalTest]
    [Component("Test Resources")]
    public class TestResourceSearchTests
    {
        /// <summary>
        /// Verifies that <see cref="FindResourceByOwner"/> correctly finds a resource with a matching owner.
        /// </summary>
        [Fact]
        public void FindResourceByOwner_WithMatchingOwner_ResourceFound()
        {
            TestResource[] resources = GetTestResources();

            var filter = new FindResourceByOwner("owner1");

            // Act
            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("name1", result[0].Name);
        }

        /// <summary>
        /// Verifies that <see cref="FindHealthyResource"/> finds all resources with a healthy status.
        /// </summary>
        [Fact]
        public void FindHealthyResource_WithTwoHealthyResources_BothResourceFound()
        {
            var resources = GetTestResources();
            resources[0].Health = HealthStatus.Healthy;
            resources[1].Health = HealthStatus.Healthy;

            var filter = new FindHealthyResource();

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that <see cref="FindHealthyResource"/> fails to find a resource when all resources are degraded.
        /// </summary>
        [Fact]
        public void FindHealthyResource_WithOnlyDegradedResources_NoResourceFound()
        {
            var resources = GetTestResources();
            resources[0].Health = HealthStatus.Degraded;
            resources[1].Health = HealthStatus.Degraded;

            var filter = new FindHealthyResource();

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="FindHealthyResource"/> finds a single healthy resource.
        /// </summary>
        [Fact]
        public void FindHealthyResource_WithOneHealthyResource_ResourceFound()
        {
            var resources = GetTestResources();
            resources[0].Health = HealthStatus.Healthy;
            resources[1].Health = HealthStatus.Unhealthy;

            var filter = new FindHealthyResource();

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Single(result);
            Assert.Equal("name1", result[0].Name);
        }

        /// <summary>
        /// Verifies that <see cref="FindResourceByLockOwner"/> finds a resource with a specific lock owner.
        /// </summary>
        [Fact]
        public void FindResourceByLockOwner_WithOneMatchingResource_ResourceFound()
        {
            var resources = GetTestResources();
            resources[0].LockOwner = "locker1";
            resources[1].LockOwner = "locker2";

            var filter = new FindResourceByLockOwner("locker1");

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Single(result);
            Assert.Equal("name1", result[0].Name);
        }

        /// <summary>
        /// Verifies that <see cref="FindEnabledResource"/> finds enabled resources.
        /// </summary>
        [Fact]
        public void FindEnabledResource_WithEnabledResource_ResourceFound()
        {
            var resources = GetTestResources();
            resources[0].Enabled = true;
            resources[1].Enabled = false;

            var filter = new FindEnabledResource();

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Single(result);
            Assert.Equal("name1", result[0].Name);
        }

        /// <summary>
        /// Verifies that <see cref="FindLockedResource"/> finds locked resources.
        /// </summary>
        [Fact]
        public void FindLockedResource_WithLockedResource_ResourceFound()
        {
            var resources = GetTestResources();
            resources[0].Locked = false;
            resources[1].Locked = true;

            var filter = new FindLockedResource();

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Single(result);
            Assert.Equal("name2", result[0].Name);
        }

        /// <summary>
        /// Verifies that <see cref="FindUnlockedResource"/> finds unlocked resources.
        /// </summary>
        [Fact]
        public void FindUnlockedResource_WithLockedResource_ResourceFound()
        {
            var resources = GetTestResources();
            resources[0].Locked = false;
            resources[1].Locked = true;

            var filter = new FindUnlockedResource();

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Single(result);
            Assert.Equal("name1", result[0].Name);
        }

        /// <summary>
        /// Verifies that <see cref="FindResourceByType"/> finds resources by type.
        /// </summary>
        [Fact]
        public void FindResourceByType_WithMatchingType_ResourceFound()
        {
            var resources = GetTestResources();

            var filter = new FindResourceByType("blender");

            var result = resources.Where(x => filter.IsMatch(x)).ToList();

            Assert.Single(result);
            Assert.Equal("name1", result[0].Name);
        }

        /// <summary>
        /// Returns a set of test resources for use in unit tests.
        /// </summary>
        /// <returns>An array of <see cref="TestResource"/> objects.</returns>
        private static TestResource[] GetTestResources()
        {
            return [new TestResource
            {
                ResourceId = "res1",
                TenantId = "tenant1",
                Name = "name1",
                Owner = "owner1",
                Types = ["blender"],
                Variables = new Dictionary<string, string>
                {
                    { "VALUE", "123" }
                }
            },
            new TestResource
            {
                ResourceId = "res2",
                Name = "name2",
                Owner = "owner2",
                Types = ["coffe-machine"],
                Variables = new Dictionary<string, string>
                {
                    { "VALUE", "234" }
                }
            }];
        }
    }
}