using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class FeatureAttribute : TestPropertyAttribute
{
    public FeatureAttribute(string feature) : base(TargetTraitNames.Feature, feature)
    {
    }
}
