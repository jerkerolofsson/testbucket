using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.XUnit;

/// <summary>
/// Defines an attribute that will add create a Component trait
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ComponentAttribute : CustomTraitAttribute
{
    public ComponentAttribute(string componentUnderTestName) :
        base(TargetTraitNames.Component, componentUnderTestName)
    {
    }
}
