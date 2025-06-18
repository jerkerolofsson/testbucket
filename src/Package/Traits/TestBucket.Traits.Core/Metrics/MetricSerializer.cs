using System.Globalization;
using System.Text.RegularExpressions;

namespace TestBucket.Traits.Core.Metrics;
public class MetricSerializer
{
    private static readonly Regex _validNameRegex = new(@"^[A-Za-z0-9_\-\.]+$");

    public static bool ValidateName(string name)
    {
        // Return false if the name contains anything else than alphanumeric characters, numbers or dashes
        if (string.IsNullOrEmpty(name))
            return false;

        // ^[A-Za-z0-9-]+$ matches only letters, numbers, and dashes
        return _validNameRegex.IsMatch(name);
    }

    public static string SerializeName(TestResultMetric metric)
    {
        ArgumentNullException.ThrowIfNull(metric);
        return $"metric:{metric.MeterName}:{metric.Name}";
    }

    public static string SerializeValue(TestResultMetric metric)
    {
        ArgumentNullException.ThrowIfNull(metric);

        if (string.IsNullOrWhiteSpace(metric.Unit))
        {
            return $"{metric.Value.ToString(CultureInfo.InvariantCulture)}";
        }
        return $"{metric.Value.ToString(CultureInfo.InvariantCulture)}{metric.Unit}";
    }

    public static string SerializeCreatedSuffix(TestResultMetric metric)
    {
        ArgumentNullException.ThrowIfNull(metric);

        string createdString = metric.Created.ToUnixTimeMilliseconds().ToString();
        return $"@{createdString}";
    }


    /// <summary>
    /// Deserializes a metric
    /// 
    /// If no unit is provided, it will be set to null.
    /// </summary>
    /// <param name="name">Format: metric:<MeterName>:<metricName></param>
    /// <param name="valueAndUnit">A floating-point value that may have a unit (for example 10.3, 10300ms or 10.3s)</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static TestResultMetric Deserialize(string name, string valueAndUnitAndDate, TimeProvider? timeProvider = null)
    {
        // Use system time provider if not specified
        timeProvider ??= TimeProvider.System;

        var items = name.Split(':');
        if (items.Length == 3 && items[0] != "metric")
        {
            throw new FormatException("Expected format: 'metric:<meterName>:<metricName>'");
        }
        else if (items.Length != 3 && items.Length != 2)
        {
            throw new FormatException("Expected format: 'metric:<meterName>:<metricName>' or '<meterName>:<metricName>'");
        }

        var metricMeterName = items[0];
        var metricName = items[1];
        if(items.Length == 3)
        {
            metricMeterName = items[1];
            metricName = items[2];
        }

        double value;
        string? unit = null;

        string valueAndUnit = valueAndUnitAndDate;

        // Parse created time
        var created = timeProvider.GetUtcNow();
        var atIndex = valueAndUnitAndDate.IndexOf('@');
        if(atIndex > 0)
        {
            valueAndUnit = valueAndUnitAndDate[0..atIndex];

            var epochString = valueAndUnitAndDate[(atIndex + 1)..].Trim();
            if(long.TryParse(epochString, out long epochs))
            {
                created = DateTimeOffset.FromUnixTimeMilliseconds(epochs);
            }
        }

        // Use regex to extract value and unit
        var match = System.Text.RegularExpressions.Regex.Match(
            valueAndUnit.Trim(),
            @"^\s*(?<value>[+-]?([0-9]*[.])?[0-9]+([eE][+-]?[0-9]+)?)\s*(?<unit>\D.*)?$"
        );

        if (!match.Success)
        {
            throw new FormatException($"Could not parse value and unit from '{valueAndUnit}'.");
        }

        if (!double.TryParse(match.Groups["value"].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value))
        {
            throw new FormatException($"Could not parse numeric value from '{valueAndUnit}'.");
        }

        unit = match.Groups["unit"].Success ? match.Groups["unit"].Value.Trim() : null;
        if (unit == string.Empty)
        {
            unit = null;
        }

        if (!ValidateName(metricMeterName))
        {
            throw new FormatException("Invalid meter name: " + metricMeterName);
        }
        if (!ValidateName(metricName))
        {
            throw new FormatException("Invalid metric name: " + metricMeterName);
        }

        return new TestResultMetric(metricMeterName, metricName, value, unit)
        {
            Created = created
        };
    }
}
