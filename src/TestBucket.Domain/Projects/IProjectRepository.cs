

using System.Runtime.CompilerServices;

using TestBucket.Contracts;

namespace TestBucket.Domain.Projects;
public interface IProjectRepository
{
    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<OneOf<TestProject, AlreadyExistsError>> AddAsync(TestProject project);

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


    /// <summary>
    /// Enumerates all items by fetching page-by-page
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<TestProject> EnumerateAsync(string tenantId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageSize = 20;
        var offset = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await SearchAsync(tenantId, new SearchQuery() { Offset = offset, Count = pageSize });
            foreach (var item in result.Items)
            {
                yield return item;
            }
            if (result.Items.Length != pageSize)
            {
                break;
            }
            offset += result.Items.Length;
        }
    }

    #region Integrations / External Systems

    /// <summary>
    /// Adds a new project integration
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="slug"></param>
    /// <param name="system"></param>
    /// <returns></returns>
    Task AddProjectIntegrationsAsync(string tenantId, string slug, ExternalSystem system);


    /// <summary>
    /// Returns all integrations for a project
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, string projectSlug);

    /// <summary>
    /// Returns all integrations for a project
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, long projectId);

    /// <summary>
    /// Saves changes made to a project integration
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="slug"></param>
    /// <param name="system"></param>
    /// <returns></returns>
    Task UpdateProjectIntegrationAsync(string tenantId, string slug, ExternalSystem system);

    /// <summary>
    /// Deletes an integration
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteProjectIntegrationAsync(string tenantId, long id);
    Task DeleteProjectAsync(TestProject project);

    #endregion Integrations / External Systems
}