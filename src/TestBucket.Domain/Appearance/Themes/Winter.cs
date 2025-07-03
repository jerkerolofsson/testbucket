using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;

/// <summary>
/// Winter is a light color theme
/// </summary>
internal class Winter : DefaultTheme
{
    public override string ToString() => "Winter";

    public Winter()
    {
        LightScheme.Base.Text = "#030303";
        LightScheme.Base.Background = "#F1EFEC";
        LightScheme.Base.Surface = "#D4C9BE";
        LightScheme.Base.DialogSurface = "#D2C1B6";
        LightScheme.Base.Dark = "#030303";

        LightScheme.Base.Primary = "#123458";
        LightScheme.Base.Secondary = "#123458";
        LightScheme.Base.Tertiary = "#123458";

        LightScheme.Input.Lines = LightScheme.Base.Surface;
        LightScheme.Input.SearchBackground = LightScheme.Base.Surface.ColorDarken(0.1);
        LightScheme.Input.SearchBackgroundHover = LightScheme.Base.Surface.ColorDarken(0.05);

        LightScheme.Field.Background = LightScheme.Base.Surface.ColorLighten(0.035);
        LightScheme.Field.BorderColor = LightScheme.Base.Surface.ColorLighten(0.1);

        DarkScheme = LightScheme;

        ChartPalette = DefaultPalettes.Cyan;
    }
}
