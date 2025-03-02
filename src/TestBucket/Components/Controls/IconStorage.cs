using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TestBucket.Components.Controls;

/// <summary>
/// https://github.com/MudBlazor/MudBlazor/blob/dev/src/MudBlazor.Docs/Pages/Features/Icons/IconStorage.cs
/// </summary>
internal class IconStorage : IEnumerable<KeyValuePair<string, Type>>
{
    private readonly IDictionary<string, Type> _icons;

    public IconStorage()
    {
        _icons = new Dictionary<string, Type>();
    }

    public Type this[string key] => _icons[key];

    public void Add(string iconType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        _icons.Add(iconType, type);
    }

    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
    {
        return _icons.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _icons.GetEnumerator();
    }
}
