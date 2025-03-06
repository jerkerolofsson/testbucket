using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Fields.Models;


[Index(nameof(TenantId),nameof(IsDeleted))]
[Table("field_definitions")]
public class FieldDefinition
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Trait. This represents the field when importing, such as a xUnit trait, or a JUnit property
    /// </summary>
    public string? Trait { get; set; }

    /// <summary>
    /// Target entity / Usage
    /// </summary>
    public FieldTarget Target { get; set; } = FieldTarget.TestCase;

    /// <summary>
    /// Value Type
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// Options that can be selected in the UI
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<string>? Options { get; set; }

    /// <summary>
    /// Description of the field
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// If true, the description is shown in the FieldEditor
    /// </summary>
    public bool ShowDescription { get; set; }

    /// <summary>
    /// Flag for soft deleted fields
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// If read only, the field can be displayed but not edited
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Can be used to save field values in the database when importing but not display the values in the UI
    /// </summary>
    public bool IsVisible { get; set; }

    // Navigation

    public string? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public long? TestProjectId { get; set; }
    public TestProject? TestProject { get; set; }
}
