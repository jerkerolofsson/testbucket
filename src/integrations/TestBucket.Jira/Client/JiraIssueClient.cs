using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

using TestBucket.Jira.Converters;
using TestBucket.Jira.Models;
using TestBucket.Jira.Serialization;

namespace TestBucket.Jira.Client;

/// <summary>
/// Provides methods for interacting with Jira issues through the Atlassian REST API.
/// This client handles individual issue retrieval, JQL-based searching, and paginated results.
/// </summary>
/// <param name="Client">The OAuth2 client used for authentication and HTTP communication</param>
internal class JiraIssueClient(JiraOauth2Client Client)
{
    /// <summary>
    /// Retrieves a single Jira issue by its resource identifier.
    /// </summary>
    /// <param name="resourceId">The unique identifier of the Jira issue to retrieve</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Jira issue if found, or null if not found.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the cloud resource is not initialized</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails</exception>
    internal async Task<JiraIssue?> GetIssueAsync(string resourceId)
    {
        // Ensure the OAuth2 client has been properly configured with a cloud resource
        Client.ThrowIfCloudResourceNotInitialized();

        var url = $"/ex/jira/{Client._cloudResource.id}/rest/api/3/issue/{resourceId}";
        return await Client._httpClient.GetFromJsonAsync<JiraIssue>(url, JiraIssueSerializer.JsonOptions);
    }

    /// <summary>
    /// Retrieves Jira issues based on JQL (Jira Query Language) with support for pagination and result limiting.
    /// This method uses async enumerable to provide efficient streaming of results without loading all data into memory.
    /// </summary>
    /// <param name="jql">The JQL query string to execute for filtering issues</param>
    /// <param name="offset">The number of issues to skip from the beginning of the result set</param>
    /// <param name="maxResults">The maximum number of issues to return (must be greater than 0)</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>An async enumerable of Jira issues matching the JQL query</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when maxResults is less than or equal to 0</exception>
    /// <exception cref="InvalidOperationException">Thrown when the cloud resource is not initialized</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled</exception>
    internal async IAsyncEnumerable<JiraIssue> GetIssuesFromJqlAsync(string jql, int offset, int maxResults, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Validate that the OAuth2 client is properly configured
        Client.ThrowIfCloudResourceNotInitialized();

        if (maxResults <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxResults), "maxResults must be greater than 0");
        }

        bool continueFetch = true;
        string? nextPageToken = null;

        // Use smaller page size to optimize API calls (max 100 per Jira API limits)
        int pageSize = Math.Min(100, maxResults);

        // Continue fetching pages until we have enough results or reach the end
        while (!cancellationToken.IsCancellationRequested && continueFetch)
        {
            // Fetch the next page of results
            var page = await GetIssuesPageFromJqlAsync(jql, pageSize, nextPageToken);
            if (page?.issues is null)
            {
                break;
            }

            IEnumerable<JiraIssue> issues = page.issues;

            // Handle offset by skipping issues from the first few pages
            if (offset > 0)
            {
                issues = page.issues.Skip(offset);
                offset -= Math.Min(offset, page.issues.Length);
            }

            // Only yield results if we've processed the offset
            if (offset == 0)
            {
                foreach (var issue in issues)
                {
                    yield return issue;

                    // Decrement the remaining count and stop if we've reached the limit
                    maxResults--;
                    if (maxResults == 0)
                    {
                        continueFetch = false;
                        break;
                    }
                }
            }

            nextPageToken = page.nextPageToken;
            if (nextPageToken is null)
            {
                continueFetch = false;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Retrieves a single page of Jira issues based on JQL query with pagination support.
    /// This method is used internally by GetIssuesFromJqlAsync for paginated retrieval.
    /// </summary>
    /// <param name="jql">The JQL query string to execute</param>
    /// <param name="maxResults">The maximum number of results to return in this page</param>
    /// <param name="nextPageToken">Optional token for retrieving the next page of results</param>
    /// <param name="fields">Comma-separated list of fields to include in the response (defaults to "*all" for all fields)</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated response with issues and pagination information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the cloud resource is not initialized</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized</exception>
    internal async Task<JiraPagedIssuesResponse<JiraIssue>?> GetIssuesPageFromJqlAsync(string jql, int maxResults, string? nextPageToken = null, string fields = "*all")
    {
        // Ensure the OAuth2 client has been properly configured with a cloud resource
        Client.ThrowIfCloudResourceNotInitialized();

        var url = $"/ex/jira/{Client._cloudResource.id}/rest/api/3/search/jql?maxResults={maxResults}&jql={WebUtility.UrlEncode(jql)}&fields={WebUtility.UrlEncode(fields)}";

        // Add pagination token if provided for subsequent pages
        if (nextPageToken is not null)
        {
            url = url + $"&nextPageToken={WebUtility.UrlEncode(nextPageToken)}";
        }

        var json = await Client._httpClient.GetStringAsync(url);
        return JiraIssueSerializer.DeserializeJson(json);
    }

    internal async Task<List<JiraIssueType>> GetIssueTypesAsync(CancellationToken cancellationToken)
    {
        // Ensure the OAuth2 client has been properly configured with a cloud resource
        Client.ThrowIfCloudResourceNotInitialized();

        var url = $"/ex/jira/{Client._cloudResource.id}/rest/api/3/issuetype";
        return await Client._httpClient.GetFromJsonAsync<List<JiraIssueType>>(url, cancellationToken) ?? [];
    }
    internal async Task<CreateIssueResponse> CreateIssueAsync(JiraIssueUpdateBean issue, CancellationToken cancellationToken)
    {
        // Ensure the OAuth2 client has been properly configured with a cloud resource
        Client.ThrowIfCloudResourceNotInitialized();

        var url = $"/ex/jira/{Client._cloudResource.id}/rest/api/3/issue";

        using var response = await Client._httpClient.PostAsJsonAsync(url, issue, cancellationToken);

        var text = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var json = JsonSerializer.Serialize(issue, JiraIssueSerializer.JsonOptions);
            throw new Exception("Failed to create issue: " + text + "\njson=\n" + json);
        }

        return JsonSerializer.Deserialize<CreateIssueResponse>(text, JiraIssueSerializer.JsonOptions)
               ?? throw new Exception("Failed to deserialize CreateIssueResponse from Jira API response");  
    }
    internal async Task UpdateIssueAsync(string key, JiraIssueUpdateBean issue, CancellationToken cancellationToken)
    {
        // Ensure the OAuth2 client has been properly configured with a cloud resource
        Client.ThrowIfCloudResourceNotInitialized();

        var url = $"/ex/jira/{Client._cloudResource.id}/rest/api/3/issue/{key}";

        using var response = await Client._httpClient.PutAsJsonAsync(url, issue, cancellationToken);

        if(!response.IsSuccessStatusCode)
        {
            var json = JsonSerializer.Serialize(issue, JiraIssueSerializer.JsonOptions);
            var text = response.Content.ReadAsStringAsync();
        }

        response.EnsureSuccessStatusCode();
    }
}