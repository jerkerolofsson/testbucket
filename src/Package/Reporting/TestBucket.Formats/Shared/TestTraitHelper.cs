using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Formats.Shared
{
    internal class TestTraitHelper
    {
        public static string GetTraitName(TestTrait attribute)
        {
            // Well known traits
            if (TraitTypeConverter.TryConvert(attribute.Type, out var name))
            {
                return name;
            }

            if (attribute.Name is not null)
            {
                return attribute.Name;
            }
            return attribute.Type.ToString();
        }
        public static TraitType GetTestTraitType(string name)
        {
            // Well known traits
            if (TraitTypeConverter.TryConvert(name, out var traitType))
            {
                return traitType.Value;
            }

            if (Enum.TryParse(typeof(TraitType), name, true, out object? enumType))
            {
                return (TraitType)enumType;
            }
            return TraitType.Custom;
        }

    }
}
