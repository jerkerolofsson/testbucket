using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.Features.Duplication;

[Feature("Duplication 1.0")]
[EnrichedTest]
[IntegrationTest]
public class DuplicateTestCaseTests(TestBucketApp App)
{
    [Fact]
    public async Task DuplicateTestCase_BothHaveTheSameDescription()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync("Duplicate Team 1");
        var project = await App.Client.Projects.AddAsync(team, "Project 1");
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, "Suite 1");

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
        return new TestCaseDto { TestCaseName = "name1", Description = "description1", TenantId = App.Tenant, TeamSlug = team, ProjectSlug = project, TestSuiteSlug = suite };
    }
}
