using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestBucket.Traits.Xunit;


/// <summary>
/// Base class for custom traits
/// </summary>
[TraitDiscoverer("TestBucket.Traits.XUnit.CustomTraitDiscoverer", "TestBucket.Traits.XUnit")]
public class CustomTraitAttribute : Attribute, ITraitAttribute
{
    public string Key { get; init; }
    public string Value { get; init; }

    public CustomTraitAttribute(string key, string value)
    {
        Key = key;
        Value = value;
    }
}