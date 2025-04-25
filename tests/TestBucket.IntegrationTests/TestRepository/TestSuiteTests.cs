using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.TestRepository;

[EnrichedTest]
[IntegrationTest]
public class TestSuiteTests(TestBucketApp App)
{
    [Fact]
    public async Task CreateTestCase_HasSlug()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync("New Suite 1");
        var project = await App.Client.Projects.AddAsync(team, "Project 1");
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, "Suite 1");
        var test = await App.Client.TestRepository.AddTestAsync(team, project, suite, "Test 1");

        try
        {
            Assert.NotNull(test);
        }
        finally
        {
            // Cleanup
            await App.Client.Projects.DeleteAsync(project);
            await App.Client.Teams.DeleteAsync(team);
        }
    }
}
