using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Search;
public static class BaseQueryParser
{
    public static readonly HashSet<string> Keywords =
        [
        "since",
        "team-id",
        "project-id",
        "from",
        "until"
        ];

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
                    break;

                case "from":
                    if(DateTimeOffset.TryParse(pair.Value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dateTimeOffsetFromCulture))
                    {
                        query.CreatedFrom = dateTimeOffsetFromCulture;
                    }
                    if (DateTimeOffset.TryParseExact(pair.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dateTimeOffsetFromIso))
                    {
                        query.CreatedFrom = dateTimeOffsetFromIso;
                    }
                    break;
                case "until":
                    if (DateTimeOffset.TryParse(pair.Value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dateTimeOffsetUntilCulture))
                    {
                        query.CreatedFrom = dateTimeOffsetUntilCulture;
                    }
                    if (DateTimeOffset.TryParseExact(pair.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dateTimeOffsetUntilIso))
                    {
                        query.CreatedFrom = dateTimeOffsetUntilIso;
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

    private static TimeSpan ParseSince(string value)
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
            default:
                throw new FormatException("Unexpected unit suffix: " + unit);
        }
        
    }
}
