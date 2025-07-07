using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Jira.Models;

namespace TestBucket.Jira.Client;
internal class JiraOauth2Client : IDisposable
{
    private readonly string _accessToken;
    internal readonly HttpClient _httpClient;
    internal CloudResource? _cloudResource;

    public JiraIssueClient Issues => new JiraIssueClient(this);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseUrl">Public base url for jira project, e.g. https://yourproject.atlassian.net</param>
    /// <param name="accessToken"></param>
    public JiraOauth2Client(string accessToken)
    {
        _accessToken = accessToken;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.atlassian.com");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

        // https://developer.atlassian.com/cloud/jira/platform/rest/v2/intro/#expansion
        _httpClient.DefaultRequestHeaders.Add("X-Atlassian-Token", "nocheck");
    }

    public static async Task<JiraOauth2Client> CreateAsync(string baseUrl, string accessToken)
    {
        var client = new JiraOauth2Client(accessToken);
        var resources = await client.GetAccessibleResourcesAsync();

        var resource = resources.Where(x => x.url == baseUrl).FirstOrDefault();
        if(resource is null)
        {
            throw new Exception($"No resource matching: '{baseUrl}' found");
        }
        client._cloudResource = resource;
        return client;
    }

    [MemberNotNull(nameof(_cloudResource))]
    internal void ThrowIfCloudResourceNotInitialized()
    {
        if (_cloudResource is null)
        {
            throw new Exception("Specified resource is not available");
        }
    }
    public async Task<List<CloudResource>> GetAccessibleResourcesAsync()
    {
        var url = "/oauth/token/accessible-resources";
        return await _httpClient.GetFromJsonAsync<List<CloudResource>>(url) ?? throw new Exception("Failed to get accessible resources"); 
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
