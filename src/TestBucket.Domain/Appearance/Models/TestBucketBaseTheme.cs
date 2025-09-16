using System.Text;

using TestBucket.Contracts.Appearance;
using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Appearance.Models;
public abstract class TestBucketBaseTheme
{
    public TypographySet TypographySet { get; set; } = new();
    public ColorScheme DarkScheme { get; set; } = new ColorScheme();
    public ColorScheme LightScheme { get; set; } = new ColorScheme();

    /// <summary>
    /// Palette used for charts
    /// </summary>
    public ThemePalette ChartPalette { get; set; } = DefaultPalettes.Default;

    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public abstract string Dark { get; }
    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public abstract string Light { get; }

    /// <summary>
    /// Generates a stylesheet for the theme and the specified color scheme 
    /// </summary>
    /// <param name="scheme"></param>
    /// <returns></returns>
    internal string GenerateStyle(ColorScheme scheme)
    {
        var css = new StringBuilder();
        css.AppendLine("body {");

        WriteTypo(css, TypographySet.Default, "default");
        WriteTypo(css, TypographySet.Body1, "body1");
        WriteTypo(css, TypographySet.Body2, "body2");
        WriteTypo(css, TypographySet.Caption, "caption");
        WriteTypo(css, TypographySet.Subtitle1, "subtitle1");
        WriteTypo(css, TypographySet.Subtitle2, "subtitle2");

        WriteTypo(css, TypographySet.H1, "h1");
        WriteTypo(css, TypographySet.H2, "h2");
        WriteTypo(css, TypographySet.H3, "h3");
        WriteTypo(css, TypographySet.H4, "h4");
        WriteTypo(css, TypographySet.H5, "h5");
        WriteTypo(css, TypographySet.H6, "h6");

        if (scheme.Field.Background is not null)
        {
            css.AppendLine($"--tb-palette-field-background: {scheme.Field.Background};");
        }
        if (scheme.Field.BorderColor is not null)
        {
            css.AppendLine($"--tb-palette-field-border: {scheme.Field.BorderColor};");
        }

        if (scheme.ContextMenu.Background is not null)
        {
            css.AppendLine($"--tb-palette-contextmenu-background: {scheme.ContextMenu.Background};");
        }
        if (scheme.Input.Background is not null)
        {
            css.AppendLine($"--tb-palette-input-background: {scheme.Input.Background};");
        }
        if (scheme.Input.BackgroundHover is not null)
        {
            css.AppendLine($"--tb-palette-input-background-hover: {scheme.Input.BackgroundHover};");
        }
        if (scheme.Input.SearchBackground is not null)
        {
            css.AppendLine($"--tb-palette-input-search-background: {scheme.Input.SearchBackground};");
        }
        if (scheme.Input.SearchBackgroundHover is not null)
        {
            css.AppendLine($"--tb-palette-input-search-background-hover: {scheme.Input.SearchBackgroundHover};");
        }
        if (scheme.Input.Lines is not null)
        {
            css.AppendLine($"--mud-palette-lines-inputs: {scheme.Input.Lines};");
        }

        if (scheme.Base.Text is not null)
        {
            css.AppendLine($"--mud-palette-text-primary: {scheme.Base.Text.ToString(ColorOutputFormats.HexA)};");
        }


        if (scheme.Base.Action is not null)
        {
            css.AppendLine($"--mud-palette-action-default: {scheme.Base.Action};");
        }
        if (scheme.Base.ActionHover is not null)
        {
            css.AppendLine($"--mud-palette-action-default-hover: {scheme.Base.ActionHover};");
        }

        if (scheme.Base.Dark is not null)
        {
            css.AppendLine($"--mud-palette-dark: {scheme.Base.Dark};");
        }
        if (scheme.Base.Surface is not null)
        {
            css.AppendLine($"--mud-palette-surface: {scheme.Base.Surface};");
            css.AppendLine($"--mud-palette-surface-darken: {scheme.Base.Surface.ColorDarken(0.1)};");
            css.AppendLine($"--mud-palette-surface-lighten: {scheme.Base.Surface.ColorLighten(0.1)};");
        }
        if (scheme.Base.DialogSurface is not null)
        {
            css.AppendLine($"--mud-palette-dialog-surface: {scheme.Base.DialogSurface};");
            css.AppendLine($"--mud-palette-dialog-surface-darken: {scheme.Base.DialogSurface.ColorDarken(0.1)};");
            css.AppendLine($"--mud-palette-dialog-surface-lighten: {scheme.Base.DialogSurface.ColorLighten(0.1)};");
        }
        else if (scheme.Base.Surface is not null)
        {
            css.AppendLine($"--mud-palette-dialog-surface: {scheme.Base.Surface};");
            css.AppendLine($"--mud-palette-dialog-surface-darken: {scheme.Base.Surface.ColorDarken(0.1)};");
            css.AppendLine($"--mud-palette-dialog-surface-lighten: {scheme.Base.Surface.ColorLighten(0.1)};");
        }
        if (scheme.Base.Background is not null)
        {
            css.AppendLine($"--mud-palette-background: {scheme.Base.Background};");
        }
        if (scheme.Base.Primary is not null)
        {
            css.AppendLine($"--mud-palette-primary: {scheme.Base.Primary};");
            css.AppendLine($"--mud-palette-primary-darken: {scheme.Base.Primary.ColorDarken(0.1)};");
            css.AppendLine($"--mud-palette-primary-lighten: {scheme.Base.Primary.ColorLighten(0.1)};");
        }
        if (scheme.Base.PrimaryText is not null)
        {
            css.AppendLine($"--mud-palette-primary-text: {scheme.Base.PrimaryText.ToString(ColorOutputFormats.HexA)};");
        }
        else if (scheme.Base.Primary is not null)
        {
            var primaryText = ContrastColorCalculator.GetContrastingTextColor(scheme.Base.Primary);
            css.AppendLine($"--mud-palette-primary-text: {primaryText.ToString(ColorOutputFormats.HexA)};");
        }


        if (scheme.Base.Secondary is not null)
        {
            css.AppendLine($"--mud-palette-secondary: {scheme.Base.Secondary};");
            css.AppendLine($"--mud-palette-secondary-darken: {scheme.Base.Secondary.ColorDarken(0.1)};");
            css.AppendLine($"--mud-palette-secondary-lighten: {scheme.Base.Secondary.ColorLighten(0.1)};");
        }

        if (scheme.Base.SecondaryText is not null)
        {
            css.AppendLine($"--mud-palette-secondary-text: {scheme.Base.SecondaryText.ToString(ColorOutputFormats.HexA)};");
        }
        else if (scheme.Base.Secondary is not null)
        {
            var primaryText = ContrastColorCalculator.GetContrastingTextColor(scheme.Base.Secondary);
            css.AppendLine($"--mud-palette-secondary-text: {primaryText.ToString(ColorOutputFormats.HexA)};");
        }

        if (scheme.Base.Tertiary is not null)
        {
            css.AppendLine($"--mud-palette-tertiary: {scheme.Base.Tertiary};");
            css.AppendLine($"--mud-palette-tertiary-darken: {scheme.Base.Tertiary.ColorDarken(0.1)};");
            css.AppendLine($"--mud-palette-tertiary-lighten: {scheme.Base.Tertiary.ColorLighten(0.1)};");
        }

        if (scheme.Base.TertiaryText is not null)
        {
            css.AppendLine($"--mud-palette-tertiary-text: {scheme.Base.TertiaryText.ToString(ColorOutputFormats.HexA)};");
        }
        else if (scheme.Base.Tertiary is not null)
        {
            var primaryText = ContrastColorCalculator.GetContrastingTextColor(scheme.Base.Tertiary);
            css.AppendLine($"--mud-palette-tertiary-text: {primaryText.ToString(ColorOutputFormats.HexA)};");
        }

        css.AppendLine("}");
        return css.ToString();
    }

    private static void WriteTypo(StringBuilder css, Typography typo, string name)
    {
        css.AppendLine($"--mud-typography-{name}-size: {typo.Size};");
        css.AppendLine($"--mud-typography-{name}-weight: {typo.Weight};");
    }
}
