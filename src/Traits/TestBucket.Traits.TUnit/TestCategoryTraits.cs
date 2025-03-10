using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.TUnit;

/// <summary>
/// Adds a custom TestCategory trait
/// </summary>
public class TestCategoryAttribute : PropertyAttribute
{
    public TestCategoryAttribute(string customCategory) :
        base(TestTraitNames.TestCategory, customCategory)
    {
    }
}

public class UnitTestAttribute : PropertyAttribute
{
    public UnitTestAttribute() :
        base(TestTraitNames.TestCategory, "Unit")
    {
    }
}

public class ApiTestAttribute : PropertyAttribute
{
    public ApiTestAttribute() :
        base(TestTraitNames.TestCategory, "API")
    {
    }
}
public class IntegrationTestAttribute : PropertyAttribute
{
    public IntegrationTestAttribute() :
        base(TestTraitNames.TestCategory, "Integration")
    {
    }
}

public class EndToEndTestAttribute : PropertyAttribute
{
    public EndToEndTestAttribute() :
        base(TestTraitNames.TestCategory, "E2E")
    {
    }
}


