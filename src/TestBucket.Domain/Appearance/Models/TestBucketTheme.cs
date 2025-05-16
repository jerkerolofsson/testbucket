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
}

public class FieldTheme
{
    public ThemeColor? Background { get; set; }
    public ThemeColor? BorderColor { get; set; }
}

public class ColorScheme
{
    public FieldTheme Field { get; set; } = new();
    public Base Base { get; set; } = new();
    public Input Input { get; set; } = new();
}

public class TestBucketTheme : TestBucketBaseTheme
{
        public ColorScheme DarkScheme { get; set; } = new ColorScheme
        {
            Base = new Base
            {
                Background = "#212126",
                Surface = "#323237",
                TextPrimary = "#f5f5ff",
            },
            Input = new Input 
            { 
                Background = "#202025",
            },
            Field = new FieldTheme
            {
                Background = "#3A3A3D",
                BorderColor = ThemeColor.Parse("#3a3a3d").ColorLighten(0.2),
            }
        };
        public ColorScheme LightScheme { get; set; } = new ColorScheme
        {
            Base = new Base
            {
                Background = "#eee",
                Surface = "#fff",
                TextPrimary = "#111",
            },
            Input = new Input
            {
                Background = "#fff",
            },
            Field = new FieldTheme
            {
                Background = "#fff",
                BorderColor = "#eee",
            }
        };

    private string GenerateStyle(ColorScheme scheme)
    {
        var css = new StringBuilder();
        css.AppendLine("body {");

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
        if(scheme.Base.TextPrimary is not null)
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
            return new TestBucketTheme();
        }
    }
}
