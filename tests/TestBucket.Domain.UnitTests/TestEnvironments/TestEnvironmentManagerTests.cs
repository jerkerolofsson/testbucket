using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NSubstitute;
using TestBucket.Domain.Environments;
using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Environments.Specifications;
using Xunit;

namespace TestBucket.Domain.UnitTests.TestEnvironments
{
    /// <summary>
    /// Contains unit tests for <see cref="TestEnvironmentManager"/>, verifying environment management logic and repository interactions.
    /// </summary>
    [Component("Test Environments")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class TestEnvironmentManagerTests
    {
        private readonly ITestEnvironmentRepository _repository = Substitute.For<ITestEnvironmentRepository>();
        private readonly TestEnvironmentManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironmentManagerTests"/> class.
        /// </summary>
        public TestEnvironmentManagerTests()
        {
            _manager = new TestEnvironmentManager(_repository);
        }

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> with the specified tenant and user name.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="name">The user name. If null, the name claim is omitted.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> instance.</returns>
        private ClaimsPrincipal CreatePrincipal(string tenantId = "tenant1", string? name = "user")
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("tenant", tenantId));
            if (name != null)
                identity.AddClaim(new Claim(ClaimTypes.Name, name));
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.AddTestEnvironmentAsync"/> sets properties and calls the repository.
        /// </summary>
        [Fact]
        public async Task AddTestEnvironmentAsync_SetsPropertiesAndCallsRepository()
        {
            var principal = CreatePrincipal();
            var env = new TestEnvironment { Name = "env1" };

            await _manager.AddTestEnvironmentAsync(principal, env);

            Assert.Equal("tenant1", env.TenantId);
            Assert.Equal("user", env.CreatedBy);
            Assert.Equal("user", env.ModifiedBy);
            Assert.True(env.Created <= DateTimeOffset.UtcNow);
            await _repository.Received(1).AddTestEnvironmentAsync(env);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.AddTestEnvironmentAsync"/> throws if the user is not authenticated.
        /// </summary>
        [Fact]
        public async Task AddTestEnvironmentAsync_ThrowsIfUserNotAuthenticated()
        {
            var principal = CreatePrincipal(name: null);
            var env = new TestEnvironment { Name = "env1" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _manager.AddTestEnvironmentAsync(principal, env));
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.DeleteTestEnvironmentAsync"/> deletes the environment if it exists.
        /// </summary>
        [Fact]
        public async Task DeleteTestEnvironmentAsync_DeletesIfExists()
        {
            var principal = CreatePrincipal();
            var env = new TestEnvironment { Id = 42, Name = "env1", TenantId = "tenant1" };
            _repository.GetTestEnvironmentByIdAsync("tenant1", 42).Returns(env);

            await _manager.DeleteTestEnvironmentAsync(principal, 42);

            await _repository.Received(1).DeleteTestEnvironmentAsync(42);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.DeleteTestEnvironmentAsync"/> does nothing if the environment does not exist.
        /// </summary>
        [Fact]
        public async Task DeleteTestEnvironmentAsync_DoesNothingIfNotExists()
        {
            var principal = CreatePrincipal();
            _repository.GetTestEnvironmentByIdAsync("tenant1", 42).Returns((TestEnvironment?)null);

            await _manager.DeleteTestEnvironmentAsync(principal, 42);

            await _repository.DidNotReceive().DeleteTestEnvironmentAsync(Arg.Any<long>());
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.GetDefaultTestEnvironmentAsync"/> returns the first environment or null.
        /// </summary>
        [Fact]
        public async Task GetDefaultTestEnvironmentAsync_ReturnsFirstOrNull()
        {
            var principal = CreatePrincipal();
            var envs = new List<TestEnvironment> { new TestEnvironment { Name = "env1" } };
            _repository.GetTestEnvironmentsAsync(Arg.Any<IEnumerable<FilterSpecification<TestEnvironment>>>()).Returns(envs);

            var result = await _manager.GetDefaultTestEnvironmentAsync(principal, 1);

            Assert.Equal("env1", result?.Name);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.GetTestEnvironmentByIdAsync"/> calls the repository and returns the environment.
        /// </summary>
        [Fact]
        public async Task GetTestEnvironmentByIdAsync_CallsRepository()
        {
            var principal = CreatePrincipal();
            var env = new TestEnvironment { Id = 42, Name = "env1", TenantId = "tenant1" };
            _repository.GetTestEnvironmentByIdAsync("tenant1", 42).Returns(env);

            var result = await _manager.GetTestEnvironmentByIdAsync(principal, 42);

            Assert.Equal(env, result);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.GetTestEnvironmentsAsync"/> calls the repository and returns environments.
        /// </summary>
        [Fact]
        public async Task GetTestEnvironmentsAsync_CallsRepository()
        {
            var principal = CreatePrincipal();
            var envs = new List<TestEnvironment> { new TestEnvironment { Name = "env1" } };
            _repository.GetTestEnvironmentsAsync("tenant1").Returns(envs);

            var result = await _manager.GetTestEnvironmentsAsync(principal);

            Assert.Single(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.GetProjectTestEnvironmentsAsync"/> calls the repository with filters.
        /// </summary>
        [Fact]
        public async Task GetProjectTestEnvironmentsAsync_CallsRepositoryWithFilters()
        {
            var principal = CreatePrincipal();
            var envs = new List<TestEnvironment> { new TestEnvironment { Name = "env1" } };
            _repository.GetTestEnvironmentsAsync(Arg.Any<IEnumerable<FilterSpecification<TestEnvironment>>>()).Returns(envs);

            var result = await _manager.GetProjectTestEnvironmentsAsync(principal, 123);

            Assert.Single(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestEnvironmentManager.UpdateTestEnvironmentAsync"/> throws if the tenant does not match.
        /// </summary>
        [Fact]
        public async Task UpdateTestEnvironmentAsync_ThrowsIfTenantMismatch()
        {
            var principal = CreatePrincipal("tenant1");
            var env = new TestEnvironment { Name = "env1", TenantId = "other" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _manager.UpdateTestEnvironmentAsync(principal, env));
        }
    }
}