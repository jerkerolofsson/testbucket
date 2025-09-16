using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;
internal class Io : DarkMoon
{
    public override string ToString() => "Io";

    public Io()
    {
        DarkScheme.Base.Primary = "#ffdd00";
        DarkScheme.Base.Secondary = "#ff7b00";
        DarkScheme.Base.Tertiary = "#f2e9e4";
        LightScheme = DarkScheme;

        ChartPalette = DefaultPalettes.Sunset;
    }
}
