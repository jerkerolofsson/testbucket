using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestBucket.Traits.Xunit;

/// <summary>
/// Discovers traits
/// </summary>
public class CustomTraitDiscoverer : ITraitDiscoverer
{
    /// <summary>
    /// Reads trait key/value and returns it
    /// </summary>
    /// <param name="traitAttribute"></param>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var attributeInfo = traitAttribute as ReflectionAttributeInfo;
        var cuustomTraitAttribute = attributeInfo?.Attribute as CustomTraitAttribute;
        var key = cuustomTraitAttribute?.Key ?? string.Empty;
        var value = cuustomTraitAttribute?.Value ?? string.Empty;
        yield return new KeyValuePair<string, string>(key, value);
    }
}