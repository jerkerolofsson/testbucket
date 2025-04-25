using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.Features.Duplication;

[Feature("Duplication 1.0")]
[EnrichedTest]
[IntegrationTest]
public class DuplicateTestCaseTests(TestBucketApp App)
{
    [Fact]
    public async Task DuplicateTestCaseAsync()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync("Duplicate Team 1");
        var project = await App.Client.Projects.AddAsync("Project 1", team);
        var test1 = await App.Client.TestRepository.AddTestAsync(CreateTestCase(team, project));

        // Act
        var test2 = await App.Client.TestRepository.DuplicateTestAsync(test1.Id);

        // Assert

        // Cleanup

        await App.Client.Projects.DeleteAsync(project);
        await App.Client.Teams.DeleteAsync(team);
    }

    private TestCaseDto CreateTestCase(string team, string project)
    {
        return new TestCaseDto { Name = "name1", Description = "description1", TenantId = App.Tenant, Team = team, Project = project };
    }
}
