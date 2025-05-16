using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Themes;

using static TestBucket.Domain.Appearance.Color;


namespace TestBucket.Domain.Appearance.Models;

public class Base
{
    public ThemeColor? Surface { get; set; }
    public ThemeColor? Background { get; set; }
    public ThemeColor? Dark { get; set; }
    public ThemeColor? Primary { get; set; }
    public ThemeColor? Secondary { get; set; }
    public ThemeColor? Tertiary { get; set; }

    public ThemeColor? TextPrimary { get; set; }

}

public class Input
{
    public ThemeColor? Background { get; set; }

    public ThemeColor? SearchBackground { get; set; }
}

public class FieldTheme
{
    public ThemeColor? Background { get; set; }
    public ThemeColor? BorderColor { get; set; }
}

public class Typography
{
    public string Size { get; set; } = "10pt";
    public string LineHeight { get; set; } = "1.167";
    public string Weight { get; set; } = "normal";
}

public class ColorScheme
{
    public FieldTheme Field { get; set; } = new();
    public Base Base { get; set; } = new();
    public Input Input { get; set; } = new();
}

public class TypographySet
{
    public Typography Default { get; set; } = new Typography() { Size = "10pt" };
    public Typography H1 { get; set; } = new Typography() { Size = "20pt", Weight = "600", LineHeight = "1.8" };
    public Typography H2 { get; set; } = new Typography() { Size = "18pt", Weight = "600", LineHeight = "1.167" };
    public Typography H3 { get; set; } = new Typography() { Size = "16pt", Weight = "600", LineHeight = "1.167" };
    public Typography H4 { get; set; } = new Typography() { Size = "14pt", Weight = "600", LineHeight = "1.167" };
    public Typography H5 { get; set; } = new Typography() { Size = "12pt", Weight = "600", LineHeight = "1.167" };
    public Typography H6 { get; set; } = new Typography() { Size = "10pt", Weight = "600", LineHeight = "1.167" };
    public Typography Body1 { get; set; } = new Typography() { Size = "10pt" };
    public Typography Body2 { get; set; } = new Typography() { Size = "10pt" };
    public Typography Button { get; set; } = new Typography() { Size = "10pt" };
    public Typography Caption { get; set; } = new Typography() { Size = "10pt" };
    public Typography Subtitle1 { get; set; } = new Typography() { Size = "10pt" };
    public Typography Subtitle2 { get; set; } = new Typography() { Size = "10pt" };
}

public class TestBucketTheme : TestBucketBaseTheme
{
    public TypographySet TypographySet { get; set; } = new();
    public ColorScheme DarkScheme { get; set; } = new ColorScheme();
    public ColorScheme LightScheme { get; set; } = new ColorScheme();

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

        if (scheme.Input.Background is not null)
        {
            css.AppendLine($"--tb-palette-input-background: {scheme.Input.Background};");
        }
        if (scheme.Input.SearchBackground is not null)
        {
            css.AppendLine($"--tb-palette-input-search-background: {scheme.Input.SearchBackground};");
        }

        if (scheme.Base.TextPrimary is not null)
        {
            css.AppendLine($"--mud-palette-text-primary: {scheme.Base.TextPrimary};");
        }
        if (scheme.Base.Dark is not null)
        {
            css.AppendLine($"--mud-palette-dark: {scheme.Base.Dark};");
        }
        if (scheme.Base.Surface is not null)
        {
            css.AppendLine($"--mud-palette-surface: {scheme.Base.Surface};");
            css.AppendLine($"--mud-palette-surface-darken: {scheme.Base.Surface.ColorDarken(0.2)};");
            css.AppendLine($"--mud-palette-surface-lighten: {scheme.Base.Surface.ColorLighten(0.2)};");
        }
        if (scheme.Base.Background is not null)
        {
            css.AppendLine($"--mud-palette-background: {scheme.Base.Background};");
        }
        if (scheme.Base.Primary is not null)
        {
            css.AppendLine($"--mud-palette-primary: {scheme.Base.Primary};");
            css.AppendLine($"--mud-palette-primary-darken: {scheme.Base.Primary.ColorDarken(0.2)};");
            css.AppendLine($"--mud-palette-primary-lighten: {scheme.Base.Primary.ColorLighten(0.2)};");
        }
        if (scheme.Base.Secondary is not null)
        {
            css.AppendLine($"--mud-palette-secondary: {scheme.Base.Secondary};");
            css.AppendLine($"--mud-palette-secondary-darken: {scheme.Base.Secondary.ColorDarken(0.2)};");
            css.AppendLine($"--mud-palette-secondary-lighten: {scheme.Base.Secondary.ColorLighten(0.2)};");
        }
        if (scheme.Base.Tertiary is not null)
        {
            css.AppendLine($"--mud-palette-tertiary: {scheme.Base.Tertiary};");
            css.AppendLine($"--mud-palette-tertiary-darken: {scheme.Base.Tertiary.ColorDarken(0.2)};");
            css.AppendLine($"--mud-palette-tertiary-lighten: {scheme.Base.Tertiary.ColorLighten(0.2)};");
        }
        css.AppendLine("}");
        return css.ToString();
    }

    private static void WriteTypo(StringBuilder css, Typography typo, string name)
    {
        css.AppendLine($"--mud-typography-{name}-size: {typo.Size};");
        css.AppendLine($"--mud-typography-{name}-weight: {typo.Weight};");
    }

    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public override string Light => GenerateStyle(LightScheme);

    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public override string Dark => GenerateStyle(DarkScheme);

    public static TestBucketTheme BlueSteel
    {
        get
        {
            return new BlueSteel();
        }
    }

    public static TestBucketTheme Default
    {
        get
        {
            return new DefaultTheme();
        }
    }
}
