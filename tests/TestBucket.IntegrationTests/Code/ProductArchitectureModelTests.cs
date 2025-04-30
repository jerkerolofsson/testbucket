using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Code.Models;

namespace TestBucket.IntegrationTests.Code
{
    [FunctionalTest]
    [EnrichedTest]
    [IntegrationTest]
    public class ProductArchitectureModelTests(TestBucketApp App)
    {
        [Feature("Architecture 1.0")]
        [Fact]
        [TestDescription("Verifies that an imported architecture model contains correct values")]
        public async Task ImportProductArchitectureAsync()
        {
            // Arrange
            var team = await App.Client.Teams.AddAsync("Team " + Guid.NewGuid().ToString());
            var project = await App.Client.Projects.AddAsync(team, "My project " + Guid.NewGuid().ToString());
            try
            {
                ProjectArchitectureModel model = CreateModel();

                // Act
                await App.Client.Architecture.ImportProductArchitectureAsync(project, model);

                // Assert
                var result = await App.Client.Architecture.GetProductArchitectureAsync(project);
                Assert.NotNull(result.Systems);
                Assert.NotNull(result.Components);
                Assert.NotNull(result.Features);
                Assert.NotNull(result.Components);

                Assert.Single(result.Systems);
                Assert.Equal(3, result.Layers.Count);
                Assert.Single(result.Components);
            }
            finally
            {
                // Cleanup
                await App.Client.Projects.DeleteAsync(project);
                await App.Client.Teams.DeleteAsync(team);
            }
        }

        private static ProjectArchitectureModel CreateModel()
        {
            return new ProjectArchitectureModel
            {
                Layers =
                    {
                        { "api", new ArchitecturalComponent() { Paths = ["/src/app1/api/**/*"]} },
                        { "domain", new ArchitecturalComponent() { Paths = ["/src/app1/domain/**/*"]} },
                        { "data", new ArchitecturalComponent() { Paths = ["/src/app1/data/**/*"]} },

                    },
                Components =
                    {
                        { "api", new ArchitecturalComponent() { Paths = ["/src/app1/api/controller1/**/*"]} },
                    },
                Features = { },
                Systems =
                    {
                        { "app1", new ArchitecturalComponent() { Paths = ["/src/app1/**/*"]} }
                    }
            };
        }
    }
}
