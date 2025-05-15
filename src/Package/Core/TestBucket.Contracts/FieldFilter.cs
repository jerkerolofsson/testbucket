using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts;
public record class FieldFilter
{
    public required long FilterDefinitionId { get; set; }

    /// <summary>
    /// Name of field definition
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The field is a boolean
    /// </summary>
    public bool? BooleanValue { get; set; }

    /// <summary>
    /// The field is an integer
    /// </summary>
    public long? LongValue { get; set; }

    /// <summary>
    /// The field is a double
    /// </summary>
    public double? DoubleValue { get; set; }

    /// <summary>
    /// The field has is a single string
    /// </summary>
    public string? StringValue { get; set; }

    /// <summary>
    /// The field has multiple strings
    /// </summary>
    public List<string>? StringValuesList { get; set; }
    public DateTimeOffset? DateTimeOffsetValue { get; set; }
    public DateOnly? DateValue { get; set; }
    public TimeSpan? TimeSpanValue { get; set; }

    public string GetValueAsString()
    {
        if (StringValue is not null)
        {
            return StringValue;
        }
        if (TimeSpanValue is not null)
        {
            return TimeSpanValue?.ToString() ?? "null";
        }
        if (DateTimeOffsetValue is not null)
        {
            return DateTimeOffsetValue?.ToString(CultureInfo.InvariantCulture) ?? "null";
        }
        if (DateValue is not null)
        {
            return DateValue?.ToString(CultureInfo.InvariantCulture) ?? "null";
        }
        if (DoubleValue is not null)
        {
            return DoubleValue?.ToString(CultureInfo.InvariantCulture) ?? "null";
        }
        if (LongValue is not null)
        {
            return LongValue?.ToString(CultureInfo.InvariantCulture) ?? "null";
        }
        if (BooleanValue is not null)
        {
            return BooleanValue == true ? "True" : "False";
        }
        return "null";
    }
}
