
namespace TestBucket.Traits.Xunit;

/// <summary>
/// Base class for custom traits
/// </summary>
public class CustomTraitAttribute : Attribute, ITraitAttribute
{
    public string Key { get; init; }
    public string Value { get; init; }

    public CustomTraitAttribute(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        return [new KeyValuePair<string, string>(Key, Value)];
    }
}