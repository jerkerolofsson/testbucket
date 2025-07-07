using System.Text.Json;

using TestBucket.Jira.Converters;
using TestBucket.Jira.Models;

namespace TestBucket.Jira.Serialization;
internal class JiraIssueSerializer
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JiraDateTimeConverter(), new NullableJiraDateTimeConverter() }
    };

    public static JiraPagedIssuesResponse<JiraIssue>? DeserializeJson(string json)
    {
        return JsonSerializer.Deserialize<JiraPagedIssuesResponse<JiraIssue>>(json, JsonOptions);
    }

}
