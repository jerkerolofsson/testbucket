using System.Security.Claims;

using TestBucket.Traits.Core;

namespace TestBucket.Domain.Projects;
public interface IProjectManager
{
    /// <summary>
    /// Deletes a project
    /// </summary>
    /// <param name="user"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    Task DeleteAsync(ClaimsPrincipal user, TestProject project);

    /// <summary>
    /// Adds a project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    Task<OneOf<TestProject, AlreadyExistsError>> AddAsync(ClaimsPrincipal principal, TestProject project);
    Task UpdateProjectAsync(ClaimsPrincipal principal, TestProject project);
    Task<PagedResult<TestProject>> BrowseTestProjectsAsync(ClaimsPrincipal principal, int offset, int count);
    Task<string[]?> GetFieldOptionsAsync(ClaimsPrincipal principal, long testProjectId, TraitType traitType, CancellationToken cancellationToken);
    Task<TestProject?> GetTestProjectByIdAsync(ClaimsPrincipal principal, long projectId);
    Task<TestProject?> GetTestProjectBySlugAsync(ClaimsPrincipal principal, string slug);

    #region Integrations
    Task DeleteProjectIntegrationAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long projectId);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, string slug);
    Task SaveProjectIntegrationAsync(ClaimsPrincipal principal, string slug, ExternalSystem system);
    #endregion Integrations
}