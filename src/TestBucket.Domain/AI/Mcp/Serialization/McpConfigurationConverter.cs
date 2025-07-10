using System.Text.Json;
using System.Text.Json.Serialization;

using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp.Serialization;

/// <summary>
/// Custom JSON converter that handles both "servers" and "mcpServers" property names for McpServerConfiguration
/// </summary>
public class McpConfigurationConverter : JsonConverter<McpServerConfiguration>
{
    public override McpServerConfiguration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected object start token");

        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var config = new McpServerConfiguration();

        // Handle inputs
        if (root.TryGetProperty("inputs", out var inputsElement))
        {
            config.Inputs = JsonSerializer.Deserialize<List<McpInput>>(inputsElement.GetRawText(), options);
        }

        // Handle servers - try "servers" first, then "mcpServers"
        if (root.TryGetProperty("servers", out var serversElement))
        {
            config.Servers = JsonSerializer.Deserialize<Dictionary<string, McpServer>>(serversElement.GetRawText(), options);
        }
        else if (root.TryGetProperty("mcpServers", out var mcpServersElement))
        {
            config.Servers = JsonSerializer.Deserialize<Dictionary<string, McpServer>>(mcpServersElement.GetRawText(), options);
        }

        return config;
    }

    public override void Write(Utf8JsonWriter writer, McpServerConfiguration value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Inputs != null)
        {
            writer.WritePropertyName("inputs");
            JsonSerializer.Serialize(writer, value.Inputs, options);
        }

        if (value.Servers != null)
        {
            writer.WritePropertyName("servers");
            JsonSerializer.Serialize(writer, value.Servers, options);
        }

        writer.WriteEndObject();
    }
}
