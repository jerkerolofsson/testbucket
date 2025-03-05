using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Fields.Models;

/// <summary>
/// Base class for custom field values mapped to projects/test suites/test cases/runs..
/// </summary>
public class FieldValue
{
    public long Id { get; set; }

    // Values (based on FieldDefinition.Type)

    public bool? BooleanValue { get; set; }
    public long? LongValue { get; set; }
    public double? DoubleValue { get; set; }
    public string? StringValue { get; set; }

    // Navigation
    public string? TenantId { get; set; }

    public Tenant? Tenant { get; set; }

    public required long FieldDefinitionId { get; set; }
    public FieldDefinition? FieldDefinition { get; set; }
}
