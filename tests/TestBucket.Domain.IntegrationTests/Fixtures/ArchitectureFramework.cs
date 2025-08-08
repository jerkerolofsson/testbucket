using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture for testing code architecture related functions
    /// </summary>
    /// <param name="Fixture"></param>
    public class ArchitectureFramework(ProjectFixture Fixture)
    {
        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public async Task AddComponentAsync(Component component)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);

            var manager = Fixture.Services.GetRequiredService<IArchitectureManager>();

            await manager.AddComponentAsync(principal, component);
        }

        /// <summary>
        /// Adds a system
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public async Task AddSystemAsync(ProductSystem system)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);

            var manager = Fixture.Services.GetRequiredService<IArchitectureManager>();

            await manager.AddSystemAsync(principal, system);
        }

        /// <summary>
        /// Adds a feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public async Task AddFeatureAsync(Feature feature)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);

            var manager = Fixture.Services.GetRequiredService<IArchitectureManager>();

            await manager.AddFeatureAsync(principal, feature);
        }
    }
}
