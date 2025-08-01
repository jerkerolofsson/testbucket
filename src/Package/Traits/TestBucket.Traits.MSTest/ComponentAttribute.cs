using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;

/// <summary>
/// Tested component
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class ComponentAttribute : TestPropertyAttribute
{
    public ComponentAttribute(string component) : base(TargetTraitNames.Component, component)
    {
    }
}
