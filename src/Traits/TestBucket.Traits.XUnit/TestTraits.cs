using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.Xunit;

/// <summary>
/// Defines an attribute that will add create a Description trait
/// </summary>
[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = true)]
public class TagAttribute : CustomTraitAttribute
{
    public TagAttribute(string tagName) :
        base(TestTraitNames.Tag, tagName)
    {
    }
}

/// <summary>
/// Defines an attribute that will add create a Description trait
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestDescriptionAttribute : CustomTraitAttribute
{
    public TestDescriptionAttribute(string description) : 
        base(TestTraitNames.TestDescription, description)
    {
    }
}

/// <summary>
/// Defines an attribute that will define a Test ID trait
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestIdAttribute : CustomTraitAttribute
{
    public TestIdAttribute(string componentUnderTestName) :
        base(TestTraitNames.TestId, componentUnderTestName)
    {
    }
}


