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
    public string[]? StringArrayValue { get; set; }

    /// <summary>
    /// Foreign key to the definition of the field
    /// </summary>
    public required long FieldDefinitionId { get; set; }

    // Navigation

    public FieldDefinition? FieldDefinition { get; set; }

    public bool HasValue()
    {
        return BooleanValue is not null ||
            StringValue is not null ||
            LongValue is not null ||
            StringArrayValue is not null ||
            DoubleValue is not null;
    }

    public void CopyTo<T>(T copy) where T : FieldValue
    {
        copy.FieldDefinitionId = FieldDefinitionId;
        copy.BooleanValue = BooleanValue;
        copy.StringValue = StringValue;
        copy.DoubleValue = DoubleValue;
        copy.LongValue = LongValue;
        copy.StringArrayValue = StringArrayValue;
        copy.TenantId = TenantId;
    }
}
