using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;
internal class DarkerMoon : DefaultTheme
{
    public override string ToString() => "Darker Moon";

    public DarkerMoon()
    {
        DarkScheme.Input.Background = "#02080A";
        DarkScheme.Input.BackgroundHover = DarkScheme.Input.Background.ColorLighten(0.1);

        DarkScheme.Input.SearchBackground = DarkScheme.Input.Background;
        DarkScheme.Input.SearchBackgroundHover = DarkScheme.Input.BackgroundHover;

        DarkScheme.Input.Lines = "#08182A";

        DarkScheme.Base.Background = "#02080A";
        DarkScheme.Base.Surface = "#040F17";
        DarkScheme.Base.DialogSurface = "#000005";
        DarkScheme.Base.Dark = DarkScheme.Base.Background;
        DarkScheme.Field.Background = "#040F17";
        DarkScheme.Field.BorderColor = "#08182A";

        LightScheme = DarkScheme;

        ChartPalette = DefaultPalettes.Cyan;
    }
}
