using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace TestBucket.Domain.Search;
public static class BaseQueryParser
{
    /// <summary>
    /// Keywords supported by all entities
    /// </summary>
    public static readonly HashSet<string> Keywords =
        [
        "since",
        "team-id",
        "project-id",
        "from",
        "until"
        ];

    private const string DateFormat1 = "yyyy-MM-dd HH:mm:ss";
    private const string DateFormat2 = "yyyy-MM-dd";

    internal static void Serialize(SearchQuery query, List<string> items)
    {
        if (query.Since is not null)
        {
            items.Add($"since:{query.Since}");
        }
        else if (query.CreatedFrom is not null)
        {
            var date = query.CreatedFrom.Value.ToString("yyyy-MM-dd HH:mm:ss");
            items.Add($"from:\"{date}\"");
        }
        if (query.CreatedUntil is not null)
        {
            var date = query.CreatedUntil.Value.ToString("yyyy-MM-dd HH:mm:ss");
            items.Add($"until:\"{date}\"");
        }

        if (query.TeamId is not null)
        {
            items.Add($"team-id:{query.TeamId}");
        }
        if (query.ProjectId is not null)
        {
            items.Add($"project-id:{query.ProjectId}");
        }
    }

    internal static bool TryParseDateTimeOffset(string value, string? format, TimeProvider provider, [NotNullWhen(true)] out DateTimeOffset? dto)
    {
        var now = provider.GetLocalNow();
        var offset = now.Offset;

        // Parse the input as DateTime (no offset)
        if (format is null)
        {
            if (DateTime.TryParse(value, out var dt))
            {
                dto = new DateTimeOffset(dt, offset);
                return true;
            }
        }
        else if(DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExact))
        {
            dto = new DateTimeOffset(dtExact, offset);
            return true;
        }
        dto = null;
        return false;
    }


    internal static void Parse(SearchQuery query, Dictionary<string, string> result, TimeProvider? provider)
    {
        provider ??= TimeProvider.System;

        foreach (var pair in result)
        {
            switch (pair.Key)
            {
                case "since":
                    var now = provider.GetLocalNow();
                    TimeSpan since = ParseSince(pair.Value);
                    query.CreatedFrom = now - since;

                    // We set the value here so we can serialize/deserialize it properly.
                    // The actual value used for filtering is CreatedFrom
                    query.Since = pair.Value;
                    break;

                case "from":
                    if(TryParseDateTimeOffset(pair.Value, null, provider, out var dateTimeOffsetFromCulture))
                    {
                        query.CreatedFrom = dateTimeOffsetFromCulture;
                    }
                    else if(TryParseDateTimeOffset(pair.Value, DateFormat1, provider, out var dateTimeOffsetFromIso))
                    {
                        query.CreatedFrom = dateTimeOffsetFromIso;
                    }
                    else if (TryParseDateTimeOffset(pair.Value, DateFormat2, provider, out var dateTimeOffsetFromIso2))
                    {
                        query.CreatedFrom = dateTimeOffsetFromIso2;
                    }
                    break;
                case "until":
                    if (TryParseDateTimeOffset(pair.Value, null, provider, out var dateTimeOffsetFromCultureUntil))
                    {
                        query.CreatedUntil = dateTimeOffsetFromCultureUntil;
                    }
                    else if (TryParseDateTimeOffset(pair.Value, DateFormat1, provider, out var dateTimeOffsetFromIsoUntil))
                    {
                        query.CreatedUntil = dateTimeOffsetFromIsoUntil;
                    }
                    else if (TryParseDateTimeOffset(pair.Value, DateFormat2, provider, out var dateTimeOffsetFromIso2Until))
                    {
                        query.CreatedUntil = dateTimeOffsetFromIso2Until;
                    }

                    break;
                case "team-id":
                    if (long.TryParse(pair.Value, out var teamId))
                    {
                        query.TeamId = teamId;
                    }
                    break;
                case "project-id":
                    if (long.TryParse(pair.Value, out var projectId))
                    {
                        query.ProjectId = projectId;
                    }
                    break;
            }
        }
    }

    public static TimeSpan ParseSince(string value)
    {
        if(value.Length == 0)
        {
            throw new FormatException("Empty string");
        }

        var unit = value[^1];
        var digits = value[0..^1];
        if(!double.TryParse(digits, CultureInfo.InvariantCulture, out var number))
        {
            throw new FormatException($"Failed to parse floating point number: '{digits}' (invariant culture expected)");
        }
        switch (unit)
        {
            case 'w':
                // Weeks
                return TimeSpan.FromDays(number * 7);

            case 'd':
                return TimeSpan.FromDays(number);

            case 'h':
                return TimeSpan.FromHours(number);
            case 'm':
                return TimeSpan.FromMinutes(number);
            case 's':
                return TimeSpan.FromSeconds(number);
            case 'y':
                return TimeSpan.FromDays(number * 365);
            default:
                throw new FormatException("Unexpected unit suffix: " + unit);
        }
        
    }
}
