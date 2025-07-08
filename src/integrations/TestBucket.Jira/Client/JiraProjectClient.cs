using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Jira.Models;

using TestBucket.Jira.Serialization;

namespace TestBucket.Jira.Client;
internal class JiraProjectClient(JiraOauth2Client Client)
{
    internal async Task<JiraPagedValuesResponse<Project>?> SeaarchProjectsAsync(string query, int maxResults, string? nextPageToken = null)
    {
        // Ensure the OAuth2 client has been properly configured with a cloud resource
        Client.ThrowIfCloudResourceNotInitialized();

        var url = $"/ex/jira/{Client._cloudResource.id}/rest/api/3/project/search?maxResults={maxResults}&query={WebUtility.UrlEncode(query)}";

        // Add pagination token if provided for subsequent pages
        if (nextPageToken is not null)
        {
            url = url + $"&nextPageToken={WebUtility.UrlEncode(nextPageToken)}";
        }

        var json = await Client._httpClient.GetStringAsync(url);
        return JiraProjectSerializer.DeserializeJson(json);
    }
}
