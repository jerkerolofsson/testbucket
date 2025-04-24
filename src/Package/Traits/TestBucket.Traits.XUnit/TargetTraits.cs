namespace TestBucket.Traits.Xunit;

/// <summary>
/// Defines an attribute that will add create a Component trait
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class ComponentAttribute : CustomTraitAttribute
{
    public ComponentAttribute(string componentUnderTestName) :
        base(TargetTraitNames.Component, componentUnderTestName)
    {
    }
}

/// <summary>
/// Defines an attribute that will add create a Feature trait describing the product feature
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class FeatureAttribute : CustomTraitAttribute
{
    public FeatureAttribute(string tagName) :
        base(TargetTraitNames.Feature, tagName)
    {
    }
}
