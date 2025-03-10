using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Traits.Xunit
{
    /// <summary>
    /// Added as attachment and not as trait
    /// These will be added to the report, but only if the test is "enriched"
    /// </summary>
    public class TraitAttachmentPropertyAttribute : Attribute
    {
        public TraitAttachmentPropertyAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
