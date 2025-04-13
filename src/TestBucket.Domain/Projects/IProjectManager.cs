using System.Security.Claims;

using TestBucket.Traits.Core;

namespace TestBucket.Domain.Projects;
public interface IProjectManager
{
    Task AddAsync(ClaimsPrincipal principal, TestProject project);
    Task<PagedResult<TestProject>> BrowseTestProjectsAsync(ClaimsPrincipal principal, int offset, int count);
    Task DeleteProjectIntegrationAsync(ClaimsPrincipal principal, long id);
    Task<string[]?> GetFieldOptionsAsync(ClaimsPrincipal principal, long testProjectId, TraitType traitType, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long projectId);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, string slug);
    Task<TestProject?> GetTestProjectByIdAsync(ClaimsPrincipal principal, long projectId);
    Task SaveProjectIntegrationAsync(ClaimsPrincipal principal, string slug, ExternalSystem system);
}