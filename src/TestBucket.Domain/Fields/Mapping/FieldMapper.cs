using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields.Mapping;
internal class FieldMapper
{
    public static IReadOnlyList<FieldDto> MapFields(IEnumerable<FieldValue> fields)
    {
        var dtos = new List<FieldDto>();

        foreach(var field in fields)
        {
            if(field.FieldDefinition is null)
            {
                continue; // Skip fields without a definition
            }

            dtos.Add(new FieldDto
            {
                Name = field.FieldDefinition.Name,
                TraitType = field.FieldDefinition?.TraitType,
                Value = field.GetValueAsString()
            });
        }

        return dtos;
    }
}
