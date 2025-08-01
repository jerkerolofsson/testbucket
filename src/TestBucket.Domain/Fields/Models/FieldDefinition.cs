using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;
using TestBucket.Contracts.Fields;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields.Models;


[Index(nameof(TenantId),nameof(IsDeleted))]
[Table("field_definitions")]
public class FieldDefinition : ProjectEntity
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
    /// Trait Name. This represents the field when importing, such as a xUnit trait, or a JUnit property
    /// </summary>
    public string? Trait { get; set; }

    /// <summary>
    /// A well-know trait type
    /// 
    /// Some traits are treated can have rules like states
    /// </summary>
    public TraitType TraitType { get; set; } = TraitType.Custom;

    /// <summary>
    /// Target entity / Usage
    /// </summary>
    public FieldTarget Target { get; set; } = FieldTarget.TestCase;

    /// <summary>
    /// Value Type
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// Inherit the field from a parent
    /// 
    /// TestSuite => TestCase 
    /// TestCase => TestCaseRun
    /// TestRun => TestCaseRun (after TestCase)
    /// 
    /// </summary>
    public bool Inherit { get; set; }

    /// <summary>
    /// Field definitions with the same value will be grouped when displayed in UI under
    /// this category
    /// </summary>
    public string? Grouping { get; set; }

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
    /// Icon for the label
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// The type of data source
    /// </summary>
    public FieldDataSourceType DataSourceType { get; set; }

    /// <summary>
    /// Data source URI (for example if external)
    /// This can also be used to limit the fields if there are multiple sources, such as by specifying the
    /// base url to an external service
    /// </summary>
    public string? DataSourceUri { get; set; }

    /// <summary>
    /// Icons for each option (only for user defined options)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? OptionIcons { get; set; }

    /// <summary>
    /// Colors for each option (only for user defined options)
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? OptionColors { get; set; }

    /// <summary>
    /// Flag for soft deleted fields
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// If read only, the field can be displayed but not edited
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// If write only, the field cannot be displayed but can be edited
    /// </summary>
    public bool WriteOnly { get; set; }

    /// <summary>
    /// Can be used to save field values in the database when importing but not display the values in the UI
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// If true, the field can be calculated using AI Classification
    /// </summary>
    public bool UseClassifier { get; set; }

    /// <summary>
    /// Fields defined by the system cannot be deleted by a user
    /// </summary>
    public bool IsDefinedBySystem { get; set; }

    /// <summary>
    /// Required permission
    /// </summary>
    public PermissionLevel RequiredPermission { get; set; } = PermissionLevel.Write;
    
    /// <summary>
    /// Where to show the field in the UI
    /// </summary>
    public Dock? Dock { get; set; }

    public override string ToString()
    {
        return $"{Name} IsVisible={IsVisible} IsDeleted={IsDeleted}";
    }

    public override int GetHashCode() => Id.GetHashCode();
    public override bool Equals(object? obj)
    {
        if(obj is FieldDefinition other)
        {
            return Id.Equals(other.Id);
        }
        return false;
    }
}
