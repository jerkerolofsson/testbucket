
using OneOf;

using TestBucket.Data.Errors;
using TestBucket.Data.Projects.Models;

namespace TestBucket.Data.Testing;
public interface IProjectRepository
{
    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(string tenantId, string name);

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
    /// Returns true if a project exists with the specified name
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> NameExistsAsync(string tenantId, string name);
    Task<PagedResult<TestProject>> SearchAsync(string tenantId, SearchQuery query);

    /// <summary>
    /// Returns true if a project exists with the specified slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> SlugExistsAsync(string tenantId, string slug);
}