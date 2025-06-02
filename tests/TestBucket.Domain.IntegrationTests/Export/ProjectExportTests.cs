using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestBucket.Contracts.Projects;
using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Export.Zip;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Domain.IntegrationTests.Export
{
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    public class ProjectExportTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that an exported project does not include sensitive details when requested
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExportProject_WithoutSensitiveDetails_ApiKeyAndAccessTokenNotIncluded()
        {
            // Arrange
            ExternalSystem integration = CreateIntegration();
            await Fixture.AddIntegrationAsync(integration);
            using var tempDir = new TempDir();

            // Act
            var exportOptions = new ExportOptions
            {
                DestinationType = ExportDestinationType.Disk,
                ExportFormat = ExportFormat.Zip,
                Destination = Path.Combine(tempDir.TempPath, "backup.zip"),
                IncludeSensitiveDetails = false,
            };
            var backupManager = Fixture.Services.GetRequiredService<IBackupManager>();
            await backupManager.CreateBackupAsync(Impersonation.Impersonate(Fixture.App.Tenant), exportOptions);

            // Assert
            var files = tempDir.GetFiles("*.zip");
            Assert.Single(files);
            Assert.True(File.Exists(exportOptions.Destination));

            using (var zipStream = File.OpenRead(files[0].FullName))
            {
                ZipImporter importer = new ZipImporter(zipStream);
                var projectDto = await importer.DeserializeEntityAsync<ProjectDto>(x => x.Id == Fixture.ProjectId.ToString() && x.Type == "project", TestContext.Current.CancellationToken);
                Assert.NotNull(projectDto);
                Assert.NotEmpty(projectDto.ExternalSystems);
                foreach(var externalSystem in projectDto.ExternalSystems)
                {
                    Assert.True(string.IsNullOrEmpty(externalSystem.ApiKey));
                    Assert.True(string.IsNullOrEmpty(externalSystem.AccessToken));
                }
            }

            // Cleanup
            await Fixture.DeleteIntegrationAsync(integration);
        }

        /// <summary>
        /// Verifies that an exported project does include sensitive details when requested
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExportProject_WithSensitiveDetails_ApiKeyAndAccessTokenNotIncluded()
        {
            // Arrange
            ExternalSystem integration = CreateIntegration();
            await Fixture.AddIntegrationAsync(integration);
            using var tempDir = new TempDir();

            // Act
            var exportOptions = new ExportOptions
            {
                DestinationType = ExportDestinationType.Disk,
                ExportFormat = ExportFormat.Zip,
                Destination = Path.Combine(tempDir.TempPath, "backup.zip"),
                IncludeSensitiveDetails = true,
            };
            var backupManager = Fixture.Services.GetRequiredService<IBackupManager>();
            await backupManager.CreateBackupAsync(Impersonation.Impersonate(Fixture.App.Tenant), exportOptions);

            // Assert
            var files = tempDir.GetFiles("*.zip");
            Assert.Single(files);
            Assert.True(File.Exists(exportOptions.Destination));

            using (var zipStream = File.OpenRead(files[0].FullName))
            {
                ZipImporter importer = new ZipImporter(zipStream);
                var projectDto = await importer.DeserializeEntityAsync<ProjectDto>(x => x.Id == Fixture.ProjectId.ToString() && x.Type == "project", TestContext.Current.CancellationToken);
                Assert.NotNull(projectDto);
                Assert.NotEmpty(projectDto.ExternalSystems);
                foreach (var externalSystem in projectDto.ExternalSystems)
                {
                    Assert.Equal(integration.ApiKey, externalSystem.ApiKey);
                    Assert.Equal(integration.AccessToken, externalSystem.AccessToken);
                }
            }

            // Cleanup
            await Fixture.DeleteIntegrationAsync(integration);
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
    }
}
