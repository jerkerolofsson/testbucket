
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Teams;
using TestBucket.Formats.Dtos;
using TestBucket.Sdk.Client.Exceptions;
using TestBucket.Sdk.Client.Extensions;

namespace TestBucket.Sdk.Client;

public class TestRunClient(HttpClient Client)
{
    /// <summary>
    /// Returns all project field definitions
    /// </summary>
    /// <param name="slug">Project slug</param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TestRunDto> AddRunAsync(string team, string project, string runName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(runName);

        var run = new TestRunDto { Project = project, Team = team, Name = runName };

        return await AddRunAsync(run);
    }

    public async Task<TestRunDto> AddRunAsync(TestRunDto run)
    {
        var response = await Client.PutAsJsonAsync("/api/runs", run);
        await response.EnsureSuccessStatusCodeAsync();

        TestRunDto result = await response.Content.ReadFromJsonAsync<TestRunDto>() ?? throw new EmptyResponseException();
        return result;
    }

    public async Task<TestCaseRunDto> AddTestCaseRunAsync(string? run, TestCaseRunDto testCaseRun)
    {
        var response = await Client.PutAsJsonAsync($"/api/runs/{run}/tests", testCaseRun);
        await response.EnsureSuccessStatusCodeAsync();

        TestCaseRunDto result = await response.Content.ReadFromJsonAsync<TestCaseRunDto>() ?? throw new EmptyResponseException();
        return result;
    }


    /// <summary>
    /// Gets all tests in a run
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IReadOnlyList<TestCaseRunDto>> GetTestCaseRunsAsync(string? slug)
    {
        return await Client.GetFromJsonAsync< IReadOnlyList<TestCaseRunDto>>($"/api/runs/{slug}/tests") ?? [];
    }

    /// <summary>
    /// duplicates a test run
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TestCaseRunDto> DuplicateAsync(string? slug)
    {
        var response = await Client.PostAsJsonAsync($"/api/runs/{slug}/duplicate", new StringContent(""));
        await response.EnsureSuccessStatusCodeAsync();

        TestCaseRunDto result = await response.Content.ReadFromJsonAsync<TestCaseRunDto>() ?? throw new EmptyResponseException();
        return result;
    }
}
