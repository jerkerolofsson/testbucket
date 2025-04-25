
using TestBucket.Contracts.Projects;
using TestBucket.Sdk.Client.Exceptions;

namespace TestBucket.Sdk.Client;

public class ProjectClient(HttpClient Client)
{
    /// <summary>
    /// Adds a project
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Slug</returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<string> AddAsync(string name, string team)
    {
        var project = new ProjectDto { Name = name, ShortName = "", Slug = "", ExternalSystems = [], Team = team };
        var createdTeam = await AddAsync(project);
        return createdTeam.Slug;
    }


    /// <summary>
    /// Adds a project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<ProjectDto> AddAsync(ProjectDto project)
    {
        var response = await Client.PutAsJsonAsync("/api/projects", project);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDto>() ?? throw new EmptyResponseException();
    }


    /// <summary>
    /// Returns a project from a slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<ProjectDto> GetAsync(string slug)
    {
        return await Client.GetFromJsonAsync<ProjectDto>($"/api/projects/{slug}") ?? throw new EmptyResponseException();
    }

    /// <summary>
    /// Deletes a project
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task DeleteAsync(string slug)
    {
        var response = await Client.DeleteAsync($"/api/projects/{slug}");
        response.EnsureSuccessStatusCode();
    }

}
