using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.TestRepository;

[EnrichedTest]
[IntegrationTest]
[FunctionalTest]
public class TestCaseTests(TestBucketApp App)
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
            Assert.NotEmpty(test);
        }
        finally
        {
            // Cleanup
            await App.Client.TestRepository.DeleteTestAsync(test);
            await App.Client.TestRepository.DeleteSuiteAsync(suite);
            await App.Client.Projects.DeleteAsync(project);
            await App.Client.Teams.DeleteAsync(team);
        }
    }
    
}
