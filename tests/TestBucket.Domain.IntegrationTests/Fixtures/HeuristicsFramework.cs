using System.Security.Claims;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>
    public class HeuristicsFramework(ProjectFixture Fixture)
    {
        internal async Task<Heuristic> AddHeuristicAsync(long projectId, ClaimsPrincipal? principal = null)
        {
            principal ??= Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IHeuristicsManager>();

            var heuristic = new Heuristic
            {
                Name = Guid.NewGuid().ToString(),
                Description = "abc",
                TestProjectId = projectId,
            };
            await manager.AddAsync(principal, heuristic);
            return heuristic;
        }

        internal async Task<Heuristic> UpdateHeuristicAsync(Heuristic heuristic)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IHeuristicsManager>();

            
            await manager.UpdateAsync(principal, heuristic);
            return heuristic;
        }

        internal async Task<IReadOnlyList<Heuristic>> GetHeuristicsAsync(long projectId)
        {
            ProjectSpecification<Heuristic>[] filters = [new FilterByProject<Heuristic>(projectId)];

            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IHeuristicsManager>();
            var result = await manager.SearchAsync(principal, filters, 0, 1000);
            return result.Items.ToList();
        }
        internal async Task DeleteHeuristicAsync(Heuristic heuristic)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var manager = Fixture.Services.GetRequiredService<IHeuristicsManager>();

            await manager.DeleteAsync(principal, heuristic);
        }
    }
}
