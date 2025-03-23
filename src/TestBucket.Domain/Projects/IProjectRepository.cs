

using TestBucket.Contracts;

namespace TestBucket.Domain.Projects;
public interface IProjectRepository
{
    Task AddProjectIntegrationsAsync(string tenantId, string slug, ExternalSystem system);

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(long? teamId, string tenantId, string name);

    /// <summary>
    /// Generates a short name
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="slug"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<string> GenerateShortNameAsync(string slug, string tenantId);

    /// <summary>
    /// Generates a slug for a name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GenerateSlug(string name);

    /// <summary>
    /// Returns a project by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<TestProject?> GetBySlugAsync(string tenantId, string slug);
    
    /// <summary>
    /// Returns a project by ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestProject?> GetProjectByIdAsync(string tenantId, long projectId);

    /// <summary>
    /// Returns all integrations for a project
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, string slug);

    /// <summary>
    /// Returns all integrations for a project
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, long projectId);

    /// <summary>
    /// Returns true if a project exists with the specified name
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> NameExistsAsync(string tenantId, string name);

    /// <summary>
    /// Searches for projects
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<TestProject>> SearchAsync(string tenantId, SearchQuery query);

    /// <summary>
    /// Returns true if a project exists with the specified slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> SlugExistsAsync(string tenantId, string slug);

    /// <summary>
    /// Saves changes to a project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task UpdateProjectAsync(TestProject project);
    Task UpdateProjectIntegrationsAsync(string tenantId, string slug, ExternalSystem system);
}