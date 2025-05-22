
namespace TestBucket.Traits.Xunit;

/// <summary>
/// Base class for custom traits
/// </summary>
public class CustomTraitAttribute : Attribute, ITraitAttribute
{
    public string Key { get; init; }
    public string Value { get; init; }

    /// <summary>
    /// Creates a new custom trait.
    /// This will be added to the result
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public CustomTraitAttribute(string key, string value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Returns all the trait set in the constructor
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        return [new KeyValuePair<string, string>(Key, Value)];
    }
}