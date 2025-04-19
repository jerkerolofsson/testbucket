using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Contracts.Fields
{
    public class FieldDefinitionDto
    {
        public string? Trait { get; set; }
        public required string Name { get; set; }
        public FieldTarget Target { get; set; }
        public DateTimeOffset Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string? ModifiedBy { get; set; }
        public FieldType Type { get; set; }
        public List<string>? Options { get; set; }
        public Dictionary<string, string>? OptionIcons { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public bool Inherit { get; set; }
        public bool UseClassifier { get; set; }
        public bool WriteOnly { get; set; }
        public bool ReadOnly { get; set; }
        public TraitType TraitType { get; set; }
        public string? ProjectSlug { get; set; }
    }
}
