using System.Text;
using TestBucket.Contracts.Appearance.Models;
using TestBucket.Domain.Appearance.Themes;


namespace TestBucket.Domain.Appearance.Models;

public class Base
{
    public ThemeColor? DialogSurface { get; set; }
    public ThemeColor? Surface { get; set; }
    public ThemeColor? Background { get; set; }
    public ThemeColor? Dark { get; set; }
    public ThemeColor? Primary { get; set; }
    public ThemeColor? Secondary { get; set; }
    public ThemeColor? Tertiary { get; set; }

    /// <summary>
    /// --mud-palette-action-default
    /// </summary>
    public ThemeColor? Action { get; set; }

    /// <summary>
    /// --mud-palette-action-default
    /// </summary>
    public ThemeColor? ActionHover { get; set; }

    public ThemeColor? PrimaryText { get; set; }
    public ThemeColor? SecondaryText { get; set; }
    public ThemeColor? TertiaryText { get; set; }

    public ThemeColor? Text { get; set; }
}

public class ContextMenu
{
    public ThemeColor? Background { get; set; }
}

public class Input
{
    public ThemeColor? Background { get; set; }
    public ThemeColor? BackgroundHover { get; set; }

    public ThemeColor? SearchBackground { get; set; }
    public ThemeColor? SearchBackgroundHover { get; set; }

    public ThemeColor? Lines { get; set; }
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
    public ContextMenu ContextMenu { get; set; } = new();
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
    public Typography Subtitle2 { get; set; } = new Typography() { Size = "8pt" };
}

public abstract class TestBucketTheme : TestBucketBaseTheme
{
    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public override string Light => GenerateStyle(LightScheme);

    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public override string Dark => GenerateStyle(DarkScheme);

    public static TestBucketTheme Material
    {
        get
        {
            return new Material();
        }
    }
    public static TestBucketTheme Retro
    {
        get
        {
            return new Retro();
        }
    }
    public static TestBucketTheme Winter
    {
        get
        {
            return new Winter();
        }
    }
    public static TestBucketTheme DarkMoon
    {
        get
        {
            return new DarkMoon();
        }
    }
    public static TestBucketTheme BlueSteel
    {
        get
        {
            return new BlueSteel();
        }
    }
    public static TestBucketTheme LeTigre
    {
        get
        {
            return new LeTigre();
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
