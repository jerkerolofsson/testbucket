namespace TestBucket.Contracts.Appearance.Models;
public class ThemePalette
{
    public required string Name { get; set; }

    /// <summary>
    /// Palette colors
    /// </summary>
    public List<ThemeColor> Colors { get; set; } = [];

    public override int GetHashCode() => Name.GetHashCode();
    public override bool Equals(object? obj)
    {
        if(obj is ThemePalette other)
        {
            return other.Name.Equals(Name);
        }

        return base.Equals(obj);
    }
}
