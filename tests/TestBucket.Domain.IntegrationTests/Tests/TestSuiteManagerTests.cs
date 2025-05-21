using Microsoft.Extensions.DependencyInjection;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.IntegrationTests.Fixtures;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    public class TestSuiteManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        [TestDescription("""
            Verifies that timestamps are updated correctly on test suites

            # Steps
            1. Set the time to 2025-05-21 07:00:00
            2. Add a test suite
            3. Verify that the Created and Modified 
            """)]
        public async Task AddTestSuite_CreatedAndMofifiedTimesUpdated()
        {
            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero));

            // Act
            var suite = await Fixture.Tests.AddSuiteAsync();

            // Assert 
            Assert.Equal(Fixture.App.TimeProvider.GetUtcNow(), suite.Created);
            Assert.Equal(Fixture.App.TimeProvider.GetUtcNow(), suite.Modified);
        }

        [Fact]
        [FunctionalTest]
        [TestDescription("""
            Verifies that timestamps are updated correctly on test suites

            # Steps
            1. Set the time to 2025-05-21 07:00:00
            2. Add a test suite
            3. Set the time to 2025-05-21 08:00:00
            4. Verify that the Created and Modified 
            """)]
        public async Task UppdateTestSuite_CreatedAndMofifiedTimesUpdated()
        {
            var created = new DateTimeOffset(2025, 5, 21, 7, 0, 0, TimeSpan.Zero);
            var updated = new DateTimeOffset(2025, 5, 21, 8, 0, 0, TimeSpan.Zero);
            Fixture.App.TimeProvider.SetTime(created);
            var suite = await Fixture.Tests.AddSuiteAsync();
            suite.Description = "abcdef";

            // Act
            Fixture.App.TimeProvider.SetTime(updated);
            await Fixture.Tests.UpdateSuiteAsync(suite);

            // Assert 
            Assert.Equal(created, suite.Created);
            Assert.Equal(updated, suite.Modified);
        }

        [Fact]
        [FunctionalTest]
        public async Task AddTestSuite_WithPermission_IsSuccess()
        {
            var principal = Fixture.App.SiteAdministrator;
            await Fixture.Tests.AddSuiteAsync(principal);
        }

        [Fact]
        [SecurityTest]
        public async Task AddTestSuite_WithOnlyReadPermission_UnauthorizedAccessExceptionThrown()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.Read);
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await Fixture.Tests.AddSuiteAsync(principal);
            });
        }

        [Fact]
        [SecurityTest]
        public async Task AddTestSuite_WithoutPermission_UnauthorizedAccessExceptionThrown()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.None);
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await Fixture.Tests.AddSuiteAsync(principal);
            });
        }

        [Fact]
        [SecurityTest]
        public async Task DeleteTestSuite_WithoutPermission_UnauthorizedAccessExceptionThrown()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.ReadWrite);
            });
            var suite = await Fixture.Tests.AddSuiteAsync(principal);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await Fixture.Tests.DeleteSuiteAsync(principal, suite);
            });
        }
    }
}
