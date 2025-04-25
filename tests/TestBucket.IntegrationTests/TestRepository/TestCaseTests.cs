using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.TestRepository;

[EnrichedTest]
[IntegrationTest]
public class TestCaseTests(TestBucketApp App)
{
    [Fact]
    public async Task CreateTestSuite_HasSlug()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync("Test Case Tests 1");
        var project = await App.Client.Projects.AddAsync(team, "Project 1");
        var slug = await App.Client.TestRepository.AddSuiteAsync(team, project, "Suite 1");

        try
        {
            Assert.NotNull(slug);
        }
        finally
        {
            // Cleanup
            await App.Client.Projects.DeleteAsync(project);
            await App.Client.Teams.DeleteAsync(team);
        }
    }
}
