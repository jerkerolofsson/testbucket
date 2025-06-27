using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Export.Handlers;

/// <summary>
/// Serializes fields for export
/// </summary>
internal static class FieldSerializer
{
    public static TestTraitCollection ToTraits<T>(this IEnumerable<T> fields) where T : FieldValue
    {
        var traits = new TestTraitCollection();
        foreach (var field in fields)
        {
            if(field.FieldDefinition is null)
            {
                continue;
            }
            traits.Traits.Add(new TestTrait
            {
                ExportType = TraitExportType.Static,
                Type = field.FieldDefinition.TraitType,
                Name = field.FieldDefinition.Name,
                Value = field.GetValueAsString()
            });
        }
        return traits;
    }
 
}
