
using System.Security.Claims;

using TestBucket.Formats.Dtos;
using TestBucket.Sdk.Client.Exceptions;
using TestBucket.Sdk.Client.Extensions;

namespace TestBucket.Sdk.Client;

public class TestRepositoryClient(HttpClient Client)
{
    #region Test Cases

    /// <summary>
    /// Adds a test case and returns the slug
    /// </summary>
    /// <param name="team">Team slug</param>
    /// <param name="project">project slug</param>
    /// <param name="suite">suite slug</param>
    /// <param name="name">Test case name</param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<string> AddTestAsync(string team, string project, string suite, string name)
    {
        var test = new TestCaseDto { TenantId = "", TestCaseName = name, TestSuiteSlug = suite, ProjectSlug = project, TeamSlug = team };
        var created = await AddTestAsync(test);
        return created.Slug ?? throw new EmptyResponseException("Test has no slug");
    }

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TestCaseDto> AddTestAsync(TestCaseDto testCase)
    {
        var response = await Client.PutAsJsonAsync("/api/testcases", testCase);
        await response.EnsureSuccessStatusCodeAsync();
        TestCaseDto result = await response.Content.ReadFromJsonAsync<TestCaseDto>() ?? throw new EmptyResponseException();
        return result;
    }

    /// <summary>
    /// Deletes a test case
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task DeleteTestAsync(string slug)
    {
        var response = await Client.DeleteAsync($"/api/testcases/{slug}");
        await response.EnsureSuccessStatusCodeAsync();
    }

    /// <summary>
    /// Gets a test case by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<TestSuiteDto> GetTestAsync(string slug)
    {
        return await Client.GetFromJsonAsync<TestSuiteDto>($"/api/testcases/{slug}") ?? throw new EmptyResponseException();
    }

    /// <summary>
    /// Clones a test case using a client (could use a different user)
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TestCaseDto> DuplicateTestAsync(HttpClient client, string slug)
    {
        var response = await client.PostAsync($"/api/testcases/{slug}/duplicate", new StringContent(""));
        await response.EnsureSuccessStatusCodeAsync();

        TestCaseDto result = await response.Content.ReadFromJsonAsync<TestCaseDto>() ?? throw new EmptyResponseException();
        return result;
    }

    /// <summary>
    /// Clones a test case
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TestCaseDto> DuplicateTestAsync(string slug)
    {
        var response = await Client.PostAsync($"/api/testcases/{slug}/duplicate", new StringContent(""));
        await response.EnsureSuccessStatusCodeAsync();

        TestCaseDto result = await response.Content.ReadFromJsonAsync<TestCaseDto>() ?? throw new EmptyResponseException();
        return result;
    }
    #endregion Test Cases

    #region Test Suites
    /// <summary>
    /// Adds a test suite with a name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="team">Team slug</param>
    /// <param name="project">Project slug</param>
    /// <returns>slug</returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<string> AddSuiteAsync(string team, string project, string name)
    {
        var suite = new TestSuiteDto { Name = name, TeamSlug = team, ProjectSlug = project };
        var created = await AddSuiteAsync(suite);
        return created.Slug ?? throw new EmptyResponseException("Slug empty");
    }

    /// <summary>
    /// Adds a test suite
    /// </summary>
    /// <param name="testSuite"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TestSuiteDto> AddSuiteAsync(TestSuiteDto testSuite)
    {
        var response = await Client.PutAsJsonAsync("/api/testsuites", testSuite);
        await response.EnsureSuccessStatusCodeAsync();

        TestSuiteDto result = await response.Content.ReadFromJsonAsync<TestSuiteDto>() ?? throw new EmptyResponseException();
        return result;
    }

    /// <summary>
    /// Gets a test suite by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<TestSuiteDto> GetSuiteAsync(string slug)
    {
        return await Client.GetFromJsonAsync<TestSuiteDto>($"/api/testsuites/{slug}") ?? throw new EmptyResponseException();
    }

    /// <summary>
    /// Deletes a test suite
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task DeleteSuiteAsync(string slug)
    {
        var response = await Client.DeleteAsync($"/api/testsuites/{slug}");
        await response.EnsureSuccessStatusCodeAsync();
    }

    #endregion Test Suites
}
