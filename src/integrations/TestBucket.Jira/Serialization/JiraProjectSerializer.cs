using System.Text.Json;

using TestBucket.Jira.Converters;
using TestBucket.Jira.Models;

namespace TestBucket.Jira.Serialization;
internal class JiraProjectSerializer
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JiraDateTimeConverter(), new NullableJiraDateTimeConverter() }
    };

    public static JiraPagedValuesResponse<Project>? DeserializeJson(string json)
    {
        return JsonSerializer.Deserialize<JiraPagedValuesResponse<Project>>(json, JsonOptions);
    }
}
