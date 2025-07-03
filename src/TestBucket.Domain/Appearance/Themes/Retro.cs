using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;

/// <summary>
/// Retro is a blue theme with beige accent color and text.
/// </summary>
internal class Retro : DefaultTheme
{
    public override string ToString() => "Retro";

    public Retro()
    {
        LightScheme.Base.Text = "#d4d0ab";
        LightScheme.Base.Background = "#123458";
        LightScheme.Base.Surface = "#15375B";
        LightScheme.Base.DialogSurface = "#113357";
        LightScheme.Base.Dark = "#030303";

        LightScheme.Base.Primary = "#d4d0ab";
        LightScheme.Base.Secondary = "#d4d0ab";
        LightScheme.Base.Tertiary = "#d4d0ab";

        LightScheme.Input.Lines = LightScheme.Base.Surface;
        LightScheme.Input.SearchBackground = LightScheme.Base.Surface.ColorDarken(0.1);
        LightScheme.Input.SearchBackgroundHover = LightScheme.Base.Surface.ColorDarken(0.05);

        LightScheme.Field.Background = LightScheme.Base.Surface.ColorLighten(0.035);
        LightScheme.Field.BorderColor = LightScheme.Base.Surface.ColorLighten(0.1);

        DarkScheme = LightScheme;

        ChartPalette = DefaultPalettes.Cyan;
    }
}
