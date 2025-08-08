using System.Collections.Generic;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    /// <summary>
    /// Tests releated to test suties
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [EnrichedTest]
    [Component("Testing")]
    public class TestSuiteManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that timestamps are updated correctly on test suites
        /// 
        /// # Steps
        /// 1. Set the time to 2025-05-21 07:00:00
        /// 2. Add a test suite
        /// 3. Verify that the Created and Modified
        /// </summary>
        /// <returns></returns>
        [Fact]
        [FunctionalTest]
        public async Task AddTestSuite_CreatedAndMofifiedTimesUpdated()
        {
            Fixture.App.TimeProvider.SetTime(new DateTimeOffset(2025, 5, 21, 0, 0, 0, TimeSpan.Zero));

            // Act
            var suite = await Fixture.Tests.AddSuiteAsync();

            // Assert 
            Assert.Equal(Fixture.App.TimeProvider.GetUtcNow(), suite.Created);
            Assert.Equal(Fixture.App.TimeProvider.GetUtcNow(), suite.Modified);
        }

        /// <summary>
        /// Verifies that timestamps are updated correctly on test suites
        /// 
        /// # Steps
        /// 1. Set the time to 2025-05-21 07:00:00
        /// 2. Add a test suite
        /// 3. Set the time to 2025-05-21 08:00:00
        /// 4. Verify that the Created and Modified
        /// </summary>
        /// <returns></returns>
        [Fact]
        [FunctionalTest]
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

        /// <summary>
        /// Verifies that a test suite can be added successfully when the user has the required permissions.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [FunctionalTest]
        public async Task AddTestSuite_WithPermission_IsSuccess()
        {
            var principal = Fixture.App.SiteAdministrator;
            await Fixture.Tests.AddSuiteAsync(principal);
        }

        /// <summary>
        /// Verifies that an UnauthorizedAccessException is thrown when trying to add a test suite
        /// </summary>
        /// <returns></returns>
        [Fact]
        [SecurityTest]
        public async Task AddTestSuite_WithOnlyReadPermission_UnauthorizedAccessExceptionThrown()
        {
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.Read);
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await Fixture.Tests.AddSuiteAsync(principal));
        }

        /// <summary>
        /// Checks that an UnauthorizedAccessException is thrown when trying to add a test suite
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Verifies that a test suite can be deleted successfully when the user has the required permissions.
        /// </summary>
        /// <returns></returns>
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
