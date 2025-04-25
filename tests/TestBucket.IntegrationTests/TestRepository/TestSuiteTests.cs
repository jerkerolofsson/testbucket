using TestBucket.Contracts.Testing.Models;

namespace TestBucket.IntegrationTests.TestRepository;

[EnrichedTest]
[IntegrationTest]
[FunctionalTest]
public class TestSuiteTests(TestBucketApp App)
{
    [TestDescription("Verifies that when adding a new test suite the slug is returned")]
    [Fact]
    public async Task CreateTestSuite_HasSlug()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var slug = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        try
        {
            Assert.NotNull(slug);
            Assert.NotEmpty(slug);
        }
        finally
        {
            // Cleanup
            await App.Client.Projects.DeleteAsync(project);
            await App.Client.Teams.DeleteAsync(team);
        }
    }

    [TestDescription("Verifies that when adding a new test suite it can be retreived by the slug")]
    [Fact]
    public async Task CreateTestSuite_CanGetAfterAdding()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var slug = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        try
        {
            Assert.NotNull(slug);
            Assert.NotEmpty(slug);
            var suite = await App.Client.TestRepository.GetSuiteAsync(slug);
            Assert.NotNull(suite);
        }
        finally
        {
            // Cleanup
            await App.Client.Projects.DeleteAsync(project);
            await App.Client.Teams.DeleteAsync(team);
        }
    }

    [TestDescription("Verifies that when adding a new test suite the slug is returned")]
    [Fact]
    public async Task DeleteTestSuite_CouldBeDeleted()
    {
        // Arrange
        var team = await App.Client.Teams.AddAsync(Guid.NewGuid().ToString());
        var project = await App.Client.Projects.AddAsync(team, Guid.NewGuid().ToString());
        var suite = await App.Client.TestRepository.AddSuiteAsync(team, project, Guid.NewGuid().ToString());

        try
        {
            Assert.NotNull(suite);
            Assert.NotEmpty(suite);
            await App.Client.TestRepository.DeleteSuiteAsync(suite);

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await App.Client.TestRepository.GetSuiteAsync(suite);
            });
        }
        finally
        {
            // Cleanup
            await App.Client.Projects.DeleteAsync(project);
            await App.Client.Teams.DeleteAsync(team);
        }
    }
}
