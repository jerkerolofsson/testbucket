

using TestBucket.Sdk.Client.Exceptions;

namespace TestBucket.Sdk.Client;

public class TestRepositoryClient(HttpClient Client)
{
    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    /// <exception cref="EmptyResponseException"></exception>
    public async Task<TestCaseDto> AddTestAsync(TestCaseDto testCase)
    {
        var response = await Client.PutAsJsonAsync("/api/testcases", testCase);
        response.EnsureSuccessStatusCode();
        TestCaseDto result = await response.Content.ReadFromJsonAsync<TestCaseDto>() ?? throw new EmptyResponseException();
        return result;
    }

    public async Task<TestCaseDto> DuplicateTestAsync(long id)
    {
        var response = await Client.PostAsync($"/api/testcases/{id}/duplicate", new StringContent(""));
        response.EnsureSuccessStatusCode();
        TestCaseDto result = await response.Content.ReadFromJsonAsync<TestCaseDto>() ?? throw new EmptyResponseException();
        return result;
    }
}
