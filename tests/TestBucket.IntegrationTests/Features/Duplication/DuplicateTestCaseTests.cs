using System.Net;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.IntegrationTests.Features.Duplication;

[Feature("Duplication")]
[Component("Testing")]
[EnrichedTest]
[IntegrationTest]
public class DuplicateTestCaseTests(TestBucketApp App)
{
    /// <summary>
    /// Verifies that a test cannot be duplicated if the user doesn't have the correct permission
    /// </summary>
    /// <returns></returns>
    [Fact]
    [SecurityTest]
    public async Task DuplicateTestCase_WithoutTestCaseWritePermission_Fails()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        var test1 = await App.Client.TestRepository.AddTestAsync(CreateTestCase(team, project, suite));
        Assert.NotNull(test1.Slug);

        var client = App.CreateClient(Impersonation.Impersonate(configure =>
        {
            configure.TenantId = App.Tenant;
            configure.Add(PermissionEntityType.TestCase, PermissionLevel.Read);
        }));

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await App.Client.TestRepository.DuplicateTestAsync(client, test1.Slug);
        });

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);

        // Cleanup

        await App.Client.Projects.DeleteAsync(project);
        await App.Client.Teams.DeleteAsync(team);
    }

    /// <summary>
    /// Verifies that a duplicated test has the same description as the test that was duplicated
    /// </summary>
    /// <returns></returns>
    [FunctionalTest]
    [Fact]
    public async Task DuplicateTestCase_BothHaveTheSameDescription()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        var test1 = await App.Client.TestRepository.AddTestAsync(CreateTestCase(team, project, suite));
        Assert.NotNull(test1.Slug);

        // Act
        var test2 = await App.Client.TestRepository.DuplicateTestAsync(test1.Slug);

        // Assert
        Assert.Equal(test1.Description, test2.Description);

        // Cleanup

        await App.Client.Projects.DeleteAsync(project);
        await App.Client.Teams.DeleteAsync(team);
    }

    private TestCaseDto CreateTestCase(string team, string project, string suite)
    {
        return new TestCaseDto { TestCaseName = "Test To Duplicate " + Guid.NewGuid().ToString(), Description = "Text to duplicate", TenantId = App.Tenant, TeamSlug = team, ProjectSlug = project, TestSuiteSlug = suite };
    }
}
