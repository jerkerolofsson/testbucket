
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Teams;
using TestBucket.Sdk.Client.Exceptions;
using TestBucket.Sdk.Client.Extensions;

namespace TestBucket.Sdk.Client;

public class FieldsClient(HttpClient Client)
{
    /// <summary>
    /// Returns all project field definitions
    /// </summary>
    /// <param name="slug">Project slug</param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<IReadOnlyList<FieldDefinitionDto>> GetProjectFieldDefinitionsAsync(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return await Client.GetFromJsonAsync<IReadOnlyList<FieldDefinitionDto>>($"/api/fields/projects/{slug}/definitions") ?? throw new EmptyResponseException();
    }

}
