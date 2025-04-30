
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Teams;
using TestBucket.Sdk.Client.Exceptions;
using TestBucket.Sdk.Client.Extensions;
using TestBucket.Contracts.Code.Models;

namespace TestBucket.Sdk.Client;

public class ArchitectureClient(HttpClient Client)
{
    /// <summary>
    /// Returns all project field definitions
    /// </summary>
    /// <param name="projectSlug">Project slug</param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task ImportProductArchitectureAsync(string projectSlug, ProjectArchitectureModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
        using var response = await Client.PutAsJsonAsync($"/api/architecture/projects/{projectSlug}/model", model) ?? throw new EmptyResponseException();
        await response.EnsureSuccessStatusCodeAsync();
    }
    public async Task<ProjectArchitectureModel> GetProductArchitectureAsync(string projectSlug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
        return await Client.GetFromJsonAsync<ProjectArchitectureModel>($"/api/architecture/projects/{projectSlug}/model") ?? throw new EmptyResponseException();
    }

}
