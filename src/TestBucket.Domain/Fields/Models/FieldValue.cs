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

    public long Id { get; set; }

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
            LongValue is not null ||
            DoubleValue is not null;
    }


    /// <summary>
    /// Copies fields this object to another fields value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="copy"></param>
    public void CopyTo<T>(T copy) where T : FieldValue
    {
        copy.FieldDefinitionId = FieldDefinitionId;
        copy.BooleanValue = BooleanValue;
        copy.StringValue = StringValue;
        copy.DoubleValue = DoubleValue;
        copy.LongValue = LongValue;
        copy.StringValuesList = StringValuesList;
        copy.TenantId = TenantId;
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

            case FieldType.Integer:
                return LongValue?.ToString() ?? "";

            default:
                throw new NotImplementedException();
        }
    }
}
