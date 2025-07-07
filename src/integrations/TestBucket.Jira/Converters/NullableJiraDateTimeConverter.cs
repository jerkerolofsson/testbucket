using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestBucket.Jira.Converters;

/// <summary>
/// Custom JSON converter for nullable DateTime fields in Jira API responses.
/// Handles the Jira date format: "2025-06-03T09:43:31.805+0800"
/// </summary>
public class NullableJiraDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly JiraDateTimeConverter DateTimeConverter = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return DateTimeConverter.Read(ref reader, typeof(DateTime), options);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            DateTimeConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}