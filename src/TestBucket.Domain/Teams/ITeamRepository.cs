
using TestBucket.Contracts;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Teams;
public interface ITeamRepository
{
    Task<OneOf<Team, AlreadyExistsError>> AddAsync(Team team);

    /// <summary>
    /// Creates a new team
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<OneOf<Team, AlreadyExistsError>> CreateAsync(string tenantId, string name);
    Task DeleteAsync(Team team);

    /// <summary>
    /// Generates a short name
    /// </summary>
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
    /// Returns a team by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<Team?> GetBySlugAsync(string tenantId, string slug);
    
    /// <summary>
    /// Returns a team by ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task<Team?> GetTeamByIdAsync(string tenantId, long teamId);

    /// <summary>
    /// Returns true if a team exists with the specified name
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> NameExistsAsync(string tenantId, string name);

    /// <summary>
    /// Searches for teams
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<Team>> SearchAsync(string tenantId, SearchQuery query);

    /// <summary>
    /// Returns true if a project exists with the specified slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> SlugExistsAsync(string tenantId, string slug);

    /// <summary>
    /// Updates a team
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task UpdateTeamAsync(Team project);
}