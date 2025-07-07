using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestBucket.Jira.Converters;

/// <summary>
/// Custom JSON converter for DateTime fields in Jira API responses.
/// Handles the Jira date format: "2025-06-03T09:43:31.805+0800"
/// </summary>
public class JiraDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] DateTimeFormats = [
        "yyyy-MM-ddTHH:mm:ss.fffzzz",     // "2025-06-03T09:43:31.805+0800"
        "yyyy-MM-ddTHH:mm:ss.ffzzz",      // "2025-06-03T09:43:31.80+0800" 
        "yyyy-MM-ddTHH:mm:ss.fzzz",       // "2025-06-03T09:43:31.8+0800"
        "yyyy-MM-ddTHH:mm:sszzz",         // "2025-06-03T09:43:31+0800"
        "yyyy-MM-ddTHH:mm:ss.fffZ",       // "2025-06-03T09:43:31.805Z"
        "yyyy-MM-ddTHH:mm:ss.ffZ",        // "2025-06-03T09:43:31.80Z"
        "yyyy-MM-ddTHH:mm:ss.fZ",         // "2025-06-03T09:43:31.8Z"
        "yyyy-MM-ddTHH:mm:ssZ",           // "2025-06-03T09:43:31Z"
        "yyyy-MM-dd"                      // "2025-06-03" (for due dates)
    ];

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (string.IsNullOrEmpty(dateString))
        {
            return default;
        }

        // Try to parse with each supported format
        foreach (var format in DateTimeFormats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        // Fallback to default parsing
        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fallbackResult))
        {
            return fallbackResult;
        }

        throw new JsonException($"Unable to convert \"{dateString}\" to DateTime using Jira date formats.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write in ISO 8601 format when serializing
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
    }
}