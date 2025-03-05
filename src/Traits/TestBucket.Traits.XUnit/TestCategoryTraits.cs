using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.XUnit;

/// <summary>
/// Adds a custom TestCategory trait
/// </summary>
[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false)]
public class TestCategoryAttribute : CustomTraitAttribute
{
    public TestCategoryAttribute(string customCategory) :
        base(TestTraitNames.TestCategory, customCategory)
    {
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class UnitTestAttribute : CustomTraitAttribute
{
    public UnitTestAttribute() :
        base(TestTraitNames.TestCategory, "Unit")
    {
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class IntegrationTestAttribute : CustomTraitAttribute
{
    public IntegrationTestAttribute() :
        base(TestTraitNames.TestCategory, "Integration")
    {
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class EndToEndTestAttribute : CustomTraitAttribute
{
    public EndToEndTestAttribute() :
        base(TestTraitNames.TestCategory, "E2E")
    {
    }
}


