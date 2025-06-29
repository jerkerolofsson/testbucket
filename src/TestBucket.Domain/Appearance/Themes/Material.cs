using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;
internal class Material : DefaultTheme
{
    public override string ToString() => "Material";

    public Material()
    {
        DarkScheme.Base.TextPrimary = "#e6e1e3";
        DarkScheme.Base.Background = "#141314";
        DarkScheme.Base.Surface = "#211f21";
        DarkScheme.Base.DialogSurface = "#110f11";
        DarkScheme.Base.Dark = DarkScheme.Base.Background;

        DarkScheme.Base.Primary = "#b066ff";
        DarkScheme.Base.Secondary = "#03dac6";
        DarkScheme.Base.Tertiary = "#bb86fc";

        DarkScheme.Input.Lines = DarkScheme.Base.Surface;
        DarkScheme.Input.SearchBackground = DarkScheme.Base.Surface.ColorDarken(0.1);
        DarkScheme.Input.SearchBackgroundHover = DarkScheme.Base.Surface.ColorDarken(0.05);

        DarkScheme.Field.Background = DarkScheme.Base.Surface.ColorDarken(0.025);
        DarkScheme.Field.BorderColor = DarkScheme.Base.Surface.ColorLighten(0.1);

        LightScheme = DarkScheme;

        ChartPalette = DefaultPalettes.Cyan;
    }
}
