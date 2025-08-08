using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.IntegrationTests.Projects
{
    /// <summary>
    /// Tests related to managing integrations
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    [Feature("Extensions")]
    [Component("Extension Management")]
    public class ProjectIntegrations(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that an integration can be added and found by project slug
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddIntegration_CanBeFoundByProjectSlug()
        {
            ExternalSystem integration = CreateIntegration();

            await Fixture.AddIntegrationAsync(integration);

            var integrations = await Fixture.GetProjectIntegrationsBySlugAsync();
            var integration2 = integrations.FirstOrDefault(x => x.Name == integration.Name);
            CompareIntegrations(integration, integration2);
        }

        /// <summary>
        /// Verifies that an integration can be added and found by project ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddIntegration_CanBeFoundByProjectId()
        {
            ExternalSystem integration = CreateIntegration();

            await Fixture.AddIntegrationAsync(integration);

            var integrations = await Fixture.GetProjectIntegrationsByIdAsync();
            var integration2 = integrations.FirstOrDefault(x => x.Name == integration.Name);
            CompareIntegrations(integration, integration2);
        }

        /// <summary>
        /// Verifies that an integration can be deleted and that it is not returned when listing
        /// integrations by project ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetProjectIntegrationsByIdAsync_AfterDeleted_IsNotReturned()
        {
            // Arrange
            ExternalSystem integration = CreateIntegration();
            await Fixture.AddIntegrationAsync(integration);

            // Act
            await Fixture.DeleteIntegrationAsync(integration);

            // Assert
            var integrations = await Fixture.GetProjectIntegrationsByIdAsync();
            var integration2 = integrations.FirstOrDefault(x => x.Name == integration.Name);
            Assert.Null(integration2);
        }

        /// <summary>
        /// Verifies that an integration can be deleted and that it is not returned when listing
        /// integrations by project slug
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetProjectIntegrationsBySlugAsync_AfterDeleted_IsNotReturned()
        {
            // Arrange
            ExternalSystem integration = CreateIntegration();
            await Fixture.AddIntegrationAsync(integration);

            // Act
            await Fixture.DeleteIntegrationAsync(integration);

            // Assert
            var integrations = await Fixture.GetProjectIntegrationsBySlugAsync();
            var integration2 = integrations.FirstOrDefault(x => x.Name == integration.Name);
            Assert.Null(integration2);
        }



        private static ExternalSystem CreateIntegration()
        {
            return new ExternalSystem
            {
                Name = "test1" + Guid.NewGuid().ToString(),
                AccessToken = "token123",
                ApiKey = "api123",
                TestResultsArtifactsPattern = "asd",
                BaseUrl = "http://localhost:1234",
                Enabled = true,
                EnabledCapabilities = Contracts.Integrations.ExternalSystemCapability.CreateIssues,
                SupportedCapabilities = Contracts.Integrations.ExternalSystemCapability.CreateIssues | Contracts.Integrations.ExternalSystemCapability.CreatePipeline
            };
        }

        private static void CompareIntegrations(ExternalSystem integration, ExternalSystem? integration2)
        {
            Assert.NotNull(integration2);
            Assert.Equal(integration.AccessToken, integration2.AccessToken);
            Assert.Equal(integration.ApiKey, integration2.ApiKey);
            Assert.Equal(integration.EnabledCapabilities, integration2.EnabledCapabilities);
            Assert.Equal(integration.SupportedCapabilities, integration2.SupportedCapabilities);
            Assert.Equal(integration.BaseUrl, integration2.BaseUrl);
            Assert.Equal(integration.Enabled, integration2.Enabled);
            Assert.Equal(integration.TestResultsArtifactsPattern, integration2.TestResultsArtifactsPattern);
        }

    }
}
