using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields.Models;

/// <summary>
/// Base class for custom field values mapped to projects/test suites/test cases/runs..
/// </summary>
public class FieldValue : Entity
{
    public static FieldValue Empty => new FieldValue { FieldDefinitionId = 0 };

    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Set to true if the field was inherited
    /// </summary>
    public bool? Inherited { get; set; }

    // Values (based on FieldDefinition.Type)

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
    /// Date
    /// </summary>
    public DateOnly? DateValue { get; set; }

    /// <summary>
    /// TimeSpan
    /// </summary>
    public TimeSpan? TimeSpanValue { get; set; }

    /// <summary>
    /// DateTimeOffset
    /// </summary>
    public DateTimeOffset? DateTimeOffsetValue { get; set; }

    /// <summary>
    /// The field has multiple strings
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<string>? StringValuesList { get; set; }

    /// <summary>
    /// Foreign key to the definition of the field
    /// </summary>
    public required long FieldDefinitionId { get; set; }

    // Navigation

    public FieldDefinition? FieldDefinition { get; set; }

    public FieldFilter ToFieldFilter()
    {
        return new FieldFilter
        {
            FilterDefinitionId = FieldDefinitionId,
            StringValuesList = this.StringValuesList,
            LongValue = this.LongValue,
            DoubleValue = this.DoubleValue,
            BooleanValue = this.BooleanValue,
            StringValue = this.StringValue,
            DateTimeOffsetValue = this.DateTimeOffsetValue,
            DateValue = this.DateValue,
            TimeSpanValue = this.TimeSpanValue,
        };
    }

    /// <summary>
    /// Returns true if the field contains any values
    /// </summary>
    /// <returns></returns>
    public bool HasValue()
    {
        bool hasArrayValue = (StringValuesList is not null && StringValuesList.Count() > 0);
        bool hasStringValue = !string.IsNullOrEmpty(StringValue);
        return BooleanValue is not null ||
            hasStringValue ||
            hasArrayValue ||
            DateValue is not null ||
            TimeSpanValue is not null ||
            DateTimeOffsetValue is not null ||
            LongValue is not null ||
            DoubleValue is not null;
    }


    /// <summary>
    /// Copies fields this object to another fields value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="copy"></param>
    public bool CopyTo<T>(T copy) where T : FieldValue
    {
        bool changed = false;

        if (copy.FieldDefinitionId != FieldDefinitionId)
        {
            changed = true;
            copy.FieldDefinitionId = FieldDefinitionId;
        }
        if (copy.BooleanValue != BooleanValue)
        {
            changed = true;
            copy.BooleanValue = BooleanValue;
        }
        if (copy.StringValue != StringValue)
        {
            changed = true;
            copy.StringValue = StringValue;
        }
        if (copy.DoubleValue != DoubleValue)
        {
            changed = true;
            copy.DoubleValue = DoubleValue;
        }
        if (copy.LongValue != LongValue)
        {
            changed = true;
            copy.LongValue = LongValue;
        }
        if (copy.StringValuesList != StringValuesList)
        {
            changed = true;
            copy.StringValuesList = StringValuesList;
        }

        if (copy.TimeSpanValue != TimeSpanValue)
        {
            changed = true;
            copy.TimeSpanValue = TimeSpanValue;
        }
        if (copy.DateValue != DateValue)
        {
            changed = true;
            copy.DateValue = DateValue;
        }
        if (copy.DateTimeOffsetValue != DateTimeOffsetValue)
        {
            changed = true;
            copy.DateTimeOffsetValue = DateTimeOffsetValue;
        }

        if (copy.TenantId != TenantId)
        {
            changed = true;
            copy.TenantId = TenantId;
        }

        return changed;
    }

    public string GetValueAsString()
    {
        if(FieldDefinition is null)
        {
            return StringValue ?? "";
        }
        switch(FieldDefinition.Type)
        {
            case FieldType.String:
            case FieldType.SingleSelection:
            case FieldType.MultiSelection:
                return StringValue ?? "";

            case FieldType.TimeSpan:
                return TimeSpanValue?.ToString() ?? "";

            case FieldType.DateTimeOffset:
                return DateTimeOffsetValue?.ToString() ?? "";

            case FieldType.DateOnly:
                return DateValue?.ToString() ?? "";

            case FieldType.Integer:
                return LongValue?.ToString() ?? "";

            default:
                throw new NotImplementedException();
        }
    }
}
