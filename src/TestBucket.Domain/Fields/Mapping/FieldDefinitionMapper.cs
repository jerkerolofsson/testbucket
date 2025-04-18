using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields.Mapping
{
    public static class FieldDefinitionMapper
    {
        public static FieldDefinitionDto ToDto(this FieldDefinition field, string projectSlug)
        {
            return new FieldDefinitionDto
            {
                ProjectSlug = projectSlug,
                Name = field.Name,
                Trait = field.Trait,
                TraitType = field.TraitType,
                Target = field.Target,
                Created = field.Created,
                CreatedBy = field.CreatedBy,
                Modified = field.Modified,
                ModifiedBy = field.ModifiedBy,
                Description = field.Description,
                Icon = field.Icon,
                OptionIcons = field.OptionIcons,
                Options = field.Options,
                Type = field.Type,
                Inherit = field.Inherit,
                UseClassifier = field.UseClassifier,
                WriteOnly = field.WriteOnly,
                ReadOnly = field.ReadOnly
            };
        }
    }
}
