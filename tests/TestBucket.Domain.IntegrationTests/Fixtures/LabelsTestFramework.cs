using TestBucket.Domain.Labels;
using TestBucket.Domain.Labels.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>
    public class LabelsTestFramework(ProjectFixture Fixture)
    {
        internal async Task<IReadOnlyList<Label>> SearchLabelsAsync(string text, int offset, int count)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ILabelManager>();
            return await manager.SearchLabelsAsync(principal, Fixture.ProjectId, text, offset, count);
        }

        internal async Task<Label> AddLabelAsync(string title)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ILabelManager>();

            var label = new Label
            {
                Title = title,
                TestProjectId = Fixture.ProjectId,
                TeamId = Fixture.TeamId,
            };
            await manager.AddLabelAsync(principal, label);
            return label;
        }

        internal async Task<Label?> GetLabelByNameAsync(string name)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ILabelManager>();
            return await manager.GetLabelByNameAsync(principal, Fixture.ProjectId, name);
        }

        /// <summary>
        /// Returns all labels
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<Label>> GetLabelsAsync()
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ILabelManager>();

            return await manager.GetLabelsAsync(principal, Fixture.ProjectId);
        }

        internal async Task DeleteLabelAsync(Label label)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ILabelManager>();
            await manager.DeleteAsync(principal, label);
        }

        internal async Task UpdateLabelAsync(Label label)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<ILabelManager>();
            await manager.UpdateLabelAsync(principal, label);
        }
    }
}
