using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.Features.Duplication;

[Feature("Duplication 1.0")]
[FunctionalTest]
[EnrichedTest]
[IntegrationTest]
public class DuplicateTestCaseTests(TestBucketApp App)
{
    [Fact]
    [TestDescription("Verifies that a duplicated test has the same description as the test that was duplicated")]
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
