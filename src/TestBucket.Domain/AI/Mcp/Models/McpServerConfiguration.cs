using System.Text.Json;
using System.Text.Json.Serialization;

using TestBucket.Domain.AI.Mcp.Serialization;

namespace TestBucket.Domain.AI.Mcp.Models;

/// <summary>
/// Content of an MCP server configuration file.
/// </summary>
[JsonConverter(typeof(McpConfigurationConverter))]
public record class McpServerConfiguration
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new McpConfigurationConverter() },
        PropertyNameCaseInsensitive = true
    };

    public static McpServerConfiguration? FromJson(string json)
    {
        return JsonSerializer.Deserialize<McpServerConfiguration>(json, SerializerOptions);
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("inputs")]
    public List<McpInput>? Inputs { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public Dictionary<string, McpServer>? Servers { get; set; }
}

public record class McpServer
{
    [JsonPropertyName("command")]
    public string? Command { get; set; }

    [JsonPropertyName("args")]
    public string[]? Args { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("cwd")]
    public string? WorkingDirectory { get; set; }

    [JsonPropertyName("headers")]
    public McpHttpHeaders? Headers { get; set; }

    [JsonPropertyName("env")]
    public McpEnvironmentVariables? Env { get; set; }

    /// <summary>
    /// This is used to identify multiple tool that have the same implementation, and multiple
    /// tools with the same name are not added to the same agent.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ToolName { get; set; }

    /// <summary>
    /// Error message if the server configuration is invalid or cannot start.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }
}

public class McpEnvironmentVariables : Dictionary<string, string>
{
}
public class McpHttpHeaders : Dictionary<string,string>
{
}

public record class McpInput
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("password")]
    public bool? Password { get; set; }
}

