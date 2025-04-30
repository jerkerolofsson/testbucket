using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Export.Handlers;
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
