using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.TUnit;

/// <summary>
/// Defines an attribute that will add create a Description trait
/// </summary>
public class TagAttribute : PropertyAttribute
{
    public TagAttribute(string tagName) :
        base(TestTraitNames.Tag, tagName)
    {
    }
}

/// <summary>
/// Defines an attribute that will add create a Description trait
/// </summary>
public class TestDescriptionAttribute : PropertyAttribute
{
    public TestDescriptionAttribute(string description) : 
        base(TestTraitNames.TestDescription, description)
    {
    }
}

/// <summary>
/// Defines an attribute that will define a Test ID trait
/// </summary>
public class TestIdAttribute : PropertyAttribute
{
    public TestIdAttribute(string componentUnderTestName) :
        base(TestTraitNames.TestId, componentUnderTestName)
    {
    }
}


