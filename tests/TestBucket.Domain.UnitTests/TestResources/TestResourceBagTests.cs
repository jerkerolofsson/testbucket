using NSubstitute;
using System.Security.Claims;
using System.Threading.Tasks;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Allocation;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.UnitTests.TestResources
{
    /// <summary>
    /// Tests for TestResourceBag
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Test Resources")]
    public class TestResourceBagTests
    {
        /// <summary>
        /// Verifies that <see cref="TestResourceBag.ResolveVariables"/> assigns environment variables correctly when adding
        /// multiple resources of the same type. The first resource will get an environment variable with index 0, the second with index 1.
        /// </summary>
        [Fact]
        public async Task ResolveVariables_WithMultipleResourcesOfSameType_AssignsVariablesCorrectly()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var manager = Substitute.For<ITestResourceManager>();
            var bag = new TestResourceBag(principal, manager);

            var resource1 = new TestResource
            {
                ResourceId = "res1",
                Name = "name1",
                Owner = "owner1",
                Types = ["phone"],
                Variables = new Dictionary<string, string>
                {
                    { "VALUE", "123" }
                }
            };
            var resource2 = new TestResource
            {
                ResourceId = "res2",
                Name = "name2",
                Owner = "owner2",
                Types = ["phone"],
                Variables = new Dictionary<string, string>
                {
                    { "VALUE", "234" }
                }
            };
            // Add resource to bag for correct indexing
            await bag.AddAsync(resource1, DateTimeOffset.UtcNow.AddDays(1), "locker");
            await bag.AddAsync(resource2, DateTimeOffset.UtcNow.AddDays(1), "locker");

            // Act
            var variables = new Dictionary<string, string>();
            bag.ResolveVariables(resource1, "api", variables);
            bag.ResolveVariables(resource2, "api", variables);

            // Assert
            Assert.Equal("123", variables["resources__phone__0__VALUE"]);
            Assert.Equal("234", variables["resources__phone__1__VALUE"]);
        }

        /// <summary>
        /// Verifies that <see cref="TestResourceBag.ResolveVariables"/> assigns environment variables correctly.
        /// </summary>
        [Fact]
        public async Task ResolveVariables_WithOneType_AssignsVariablesCorrectly()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var manager = Substitute.For<ITestResourceManager>();
            var bag = new TestResourceBag(principal, manager);

            var resource = new TestResource
            {
                ResourceId = "res1",
                Name = "name1",
                Owner = "owner1",
                Types = ["phone" ],
                Variables = new Dictionary<string, string>
                {
                    { "URL", "http://localhost" },
                    { "TOKEN", "abc123" }
                }
            };
            // Add resource to bag for correct indexing
            await bag.AddAsync(resource, DateTimeOffset.UtcNow.AddDays(1), "locker");

            // Act
            var variables = new Dictionary<string, string>();
            bag.ResolveVariables(resource, "api", variables);

            // Assert
            Assert.Equal("http://localhost", variables["resources__phone__0__URL"]);
            Assert.Equal("abc123", variables["resources__phone__0__TOKEN"]);
        }

        /// <summary>
        /// Verifies that <see cref="TestResourceBag.ResolveVariables"/> assigns environment variables correctly.
        /// </summary>
        [Fact]
        public async Task ResolveVariables_WithMultipleTypes_AssignsVariablesCorrectly()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var manager = Substitute.For<ITestResourceManager>();
            var bag = new TestResourceBag(principal, manager);

            var resource = new TestResource
            {
                ResourceId = "res1",
                Name = "name1",
                Owner = "owner1",
                Types = ["phone", "calculator"],
                Variables = new Dictionary<string, string>
                {
                    { "URL", "http://localhost" },
                    { "TOKEN", "abc123" }
                }
            };
            // Add resource to bag for correct indexing
            await bag.AddAsync(resource, DateTimeOffset.UtcNow.AddDays(1), "locker");

            // Act
            var variables = new Dictionary<string, string>();
            bag.ResolveVariables(resource, "api", variables);

            // Assert
            Assert.Equal("http://localhost", variables["resources__phone__0__URL"]);
            Assert.Equal("http://localhost", variables["resources__calculator__0__URL"]);
            Assert.Equal("abc123", variables["resources__phone__0__TOKEN"]);
            Assert.Equal("abc123", variables["resources__calculator__0__TOKEN"]);
        }

    }


}
