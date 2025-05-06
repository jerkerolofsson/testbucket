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
    public class TestSuiteManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [FunctionalTest]
        public async Task AddTestSuite_WithPermission_IsSuccess()
        {
            using var scope = Fixture.Services.CreateScope();   
            var manager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();
            var principal = Fixture.App.SiteAdministrator;

            var testSuite = new TestSuite { Name = Guid.NewGuid().ToString(), TestProjectId = Fixture.ProjectId, TeamId = Fixture.TeamId };

            await manager.AddTestSuiteAsync(principal, testSuite);
        }

        [Fact]
        [SecurityTest]
        public async Task AddTestSuite_WithOnlyReadPermission_UnauthorizedAccessExceptionThrown()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.Read);
            });

            var testSuite = new TestSuite { Name = Guid.NewGuid().ToString() };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.AddTestSuiteAsync(principal, testSuite);
            });
        }

        [Fact]
        [SecurityTest]
        public async Task AddTestSuite_WithoutPermission_UnauthorizedAccessExceptionThrown()
        {
            using var scope = Fixture.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<ITestSuiteManager>();
            var principal = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.Add(PermissionEntityType.TestSuite, PermissionLevel.None);
            });

            var testSuite = new TestSuite { Name = Guid.NewGuid().ToString() };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await manager.AddTestSuiteAsync(principal, testSuite);
            });
        }
    }
}
