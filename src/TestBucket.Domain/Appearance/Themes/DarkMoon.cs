namespace TestBucket.Domain.Appearance.Themes;
internal class DarkMoon : DefaultTheme
{
    public override string ToString() => "Dark Moon";

    public DarkMoon()
    {
        DarkScheme.Base.TextPrimary = "#ecf2f8";
        DarkScheme.Base.Background = "#0d1117";
        DarkScheme.Base.Surface = "#161b22";
        DarkScheme.Base.DialogSurface = "#060b12";
        DarkScheme.Base.Dark = DarkScheme.Base.Background;

        DarkScheme.Base.Primary = "#03dac6";
        DarkScheme.Base.Secondary = "#cea5fb";
        DarkScheme.Base.Tertiary = "#04eab6";

        DarkScheme.Input.Lines = DarkScheme.Base.Surface;
        DarkScheme.Input.SearchBackground = DarkScheme.Base.Surface.ColorDarken(0.1);
        DarkScheme.Input.SearchBackgroundHover = DarkScheme.Base.Surface.ColorDarken(0.05);

        DarkScheme.Field.Background = DarkScheme.Base.Surface.ColorLighten(0.035);
        DarkScheme.Field.BorderColor = DarkScheme.Base.Surface.ColorLighten(0.1);

        LightScheme = DarkScheme;

        ChartPalette = DefaultPalettes.Cyan;
    }
}
