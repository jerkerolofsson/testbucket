using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;
internal class Ganymede : DarkMoon
{
    public override string ToString() => "Ganymede";

    public Ganymede()
    {
        DarkScheme.Base.Primary = "#ccff33";
        DarkScheme.Base.Secondary = "#9ef01a";
        DarkScheme.Base.Tertiary = "#70e000";
        LightScheme = DarkScheme;

        ChartPalette = DefaultPalettes.BrightGreen;
    }
}
