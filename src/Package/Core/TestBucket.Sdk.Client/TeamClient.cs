
using TestBucket.Contracts.Teams;
using TestBucket.Sdk.Client.Exceptions;

namespace TestBucket.Sdk.Client;

public class TeamClient(HttpClient Client)
{
    /// <summary>
    /// Adds a team
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Slug</returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<string> AddAsync(string name)
    {
        var team = new TeamDto { Name = name, ShortName = "", Slug = "" };
        var createdTeam = await AddAsync(team);
        return createdTeam.Slug;
    }


    /// <summary>
    /// Adds a team
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TeamDto> AddAsync(TeamDto team)
    {
        var response = await Client.PutAsJsonAsync("/api/teams", team);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TeamDto>() ?? throw new EmptyResponseException();
    }


    /// <summary>
    /// Returns a team from a slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TeamDto> GetAsync(string slug)
    {
        return await Client.GetFromJsonAsync<TeamDto>($"/api/teams/{slug}") ?? throw new EmptyResponseException();
    }

    /// <summary>
    /// Deletes a team
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task DeleteAsync(string slug)
    {
        var response = await Client.DeleteAsync($"/api/teams/{slug}");
        response.EnsureSuccessStatusCode();
    }

}
