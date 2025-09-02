using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.UnitTests.Appearance.Themes;

/// <summary>
/// Contains unit tests for the <see cref="ThemeColor"/> class, verifying color construction, parsing, equality, and manipulation.
/// </summary>
[UnitTest]
[FunctionalTest]
[Feature("Themes")]
[Component("Appearance")]
[EnrichedTest]
public class ThemeColorTests
{
    /// <summary>
    /// Tests that the default constructor creates a black color with full opacity.
    /// </summary>
    [Fact]
    public void DefaultConstructor_ShouldBeBlackWithFullOpacity()
    {
        var color = new ThemeColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
        Assert.Equal("#000000ff", color.Value);
    }

    /// <summary>
    /// Tests that the RGBA constructor sets the color properties correctly.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    [Theory]
    [InlineData(255, 128, 64, 200)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(255, 255, 255, 255)]
    public void RgbaConstructor_ShouldSetProperties(byte r, byte g, byte b, byte a)
    {
        var color = new ThemeColor(r, g, b, a);
        Assert.Equal(r, color.R);
        Assert.Equal(g, color.G);
        Assert.Equal(b, color.B);
        Assert.Equal(a, color.A);
    }

    /// <summary>
    /// Tests that the string constructor parses various color formats correctly.
    /// </summary>
    /// <param name="input">Input color string.</param>
    /// <param name="r">Expected red component.</param>
    /// <param name="g">Expected green component.</param>
    /// <param name="b">Expected blue component.</param>
    /// <param name="a">Expected alpha component.</param>
    [Theory]
    [InlineData("#ff0000", 255, 0, 0, 255)]
    [InlineData("#00ff00ff", 0, 255, 0, 255)]
    [InlineData("rgb(12,34,56)", 12, 34, 56, 255)]
    [InlineData("rgba(12,34,56,0.5)", 12, 34, 56, 127)]
    public void StringConstructor_ShouldParseColor(string input, byte r, byte g, byte b, byte a)
    {
        var color = new ThemeColor(input);
        Assert.Equal(r, color.R);
        Assert.Equal(g, color.G);
        Assert.Equal(b, color.B);
        Assert.Equal(a, color.A);
    }

    /// <summary>
    /// Tests that Themecolor.Parse returns the expected color.
    /// </summary>
    [Fact]
    public void Parse_ShouldReturnExpectedColor()
    {
        var color = ThemeColor.Parse("#11223344");
        Assert.Equal(0x11, color.R);
        Assert.Equal(0x22, color.G);
        Assert.Equal(0x33, color.B);
        Assert.Equal(0x44, color.A);
    }

    /// <summary>
    /// Tests that <see cref="ThemeColor.TryParse(string, out ThemeColor)"/> returns true and the correct color for valid input.
    /// </summary>
    [Fact]
    public void TryParse_ValidString_ShouldReturnTrueAndColor()
    {
        var result = ThemeColor.TryParse("rgba(1,2,3,0.5)", out var color);
        Assert.True(result);
        Assert.NotNull(color);
        Assert.Equal(1, color.R);
        Assert.Equal(2, color.G);
        Assert.Equal(3, color.B);
        Assert.Equal(127, color.A);
    }

    /// <summary>
    /// Tests that <see cref="ThemeColor.TryParse(string, out ThemeColor)"/> returns false for invalid input.
    /// </summary>
    [Fact]
    public void TryParse_InvalidString_ShouldReturnFalse()
    {
        var result = ThemeColor.TryParse("notacolor", out var color);
        Assert.False(result);
        Assert.Null(color);
    }

    /// <summary>
    /// Tests that the string constructor parses HTML color names correctly.
    /// </summary>
    /// <param name="colorName">HTML color name.</param>
    /// <param name="r">Expected red component.</param>
    /// <param name="g">Expected green component.</param>
    /// <param name="b">Expected blue component.</param>
    /// <param name="a">Expected alpha component.</param>
    [Theory]
    [InlineData("Red", 255, 0, 0, 255)]
    [InlineData("Green", 0, 128, 0, 255)]
    [InlineData("Blue", 0, 0, 255, 255)]
    [InlineData("Black", 0, 0, 0, 255)]
    [InlineData("White", 255, 255, 255, 255)]
    [InlineData("Yellow", 255, 255, 0, 255)]
    [InlineData("Cyan", 0, 255, 255, 255)]
    [InlineData("Magenta", 255, 0, 255, 255)]
    [InlineData("Gray", 128, 128, 128, 255)]
    [InlineData("Orange", 255, 165, 0, 255)]
    [InlineData("YellowGreen", 154, 205, 50, 255)]
    [InlineData("Purple", 128, 0, 128, 255)]
    public void StringConstructor_ShouldParseHtmlColorName(string colorName, byte r, byte g, byte b, byte a)
    {
        var color = new ThemeColor(colorName);
        Assert.Equal(r, color.R);
        Assert.Equal(g, color.G);
        Assert.Equal(b, color.B);
        Assert.Equal(a, color.A);
    }

    /// <summary>
    /// Verifies that parsing HTML color names is case-insensitive.
    /// </summary>
    /// <param name="colorName"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    [InlineData("yellowgreen", 154, 205, 50, 255)]
    [InlineData("YELLOWGReeN", 154, 205, 50, 255)]
    [Theory]
    public void StringConstructor_WithDifferentCase_ShouldParseHtmlColorName(string colorName, byte r, byte g, byte b, byte a)
    {
        var color = new ThemeColor(colorName);
        Assert.Equal(r, color.R);
        Assert.Equal(g, color.G);
        Assert.Equal(b, color.B);
        Assert.Equal(a, color.A);
    }

    /// <summary>
    /// Tests equality operations for identical <see cref="ThemeColor"/> instances.
    /// </summary>
    [Fact]
    public void Equality_ShouldWorkForIdenticalColors()
    {
        var c1 = new ThemeColor(10, 20, 30, 40);
        var c2 = new ThemeColor(10, 20, 30, 40);
        Assert.True(c1.Equals(c2));
        Assert.True(c1 == c2);
        Assert.False(c1 != c2);
    }

    /// <summary>
    /// Tests that setter methods return new instances with the changed color component.
    /// </summary>
    [Fact]
    public void Setters_ShouldReturnNewInstanceWithChangedComponent()
    {
        var color = new ThemeColor(100, 150, 200, 255);
        var newR = color.SetR(50);
        Assert.Equal(50, newR.R);
        Assert.Equal(color.G, newR.G);
        Assert.Equal(color.B, newR.B);
        Assert.Equal(color.A, newR.A);

        var newG = color.SetG(60);
        Assert.Equal(60, newG.G);

        var newB = color.SetB(70);
        Assert.Equal(70, newB.B);

        var newAlpha = color.SetAlpha(128);
        Assert.Equal(128, newAlpha.A);
    }

    /// <summary>
    /// Tests that <see cref="ThemeColor.ColorLighten(double)"/> and <see cref="ThemeColor.ColorDarken(double)"/> change the lightness as expected.
    /// </summary>
    [Fact]
    public void ColorLightenAndDarken_ShouldChangeLightness()
    {
        var color = new ThemeColor(0, 1.0, 0.5, 255);
        var lighter = color.ColorLighten(0.1);
        Assert.InRange(lighter.L, 0.59, 0.61);

        var darker = color.ColorDarken(0.1);
        Assert.InRange(darker.L, 0.39, 0.41);
    }

    /// <summary>
    /// Tests that <see cref="ThemeColor.ToString(ColorOutputFormats)"/> returns the expected string formats.
    /// </summary>
    [Fact]
    public void ToString_ShouldReturnExpectedFormats()
    {
        var color = new ThemeColor(255, 128, 64, 200);
        Assert.Equal("#ff8040", color.ToString(ColorOutputFormats.Hex));
        Assert.Equal("#ff8040c8", color.ToString(ColorOutputFormats.HexA));
        Assert.Equal("rgb(255,128,64)", color.ToString(ColorOutputFormats.RGB));
        Assert.StartsWith("rgba(255,128,64,", color.ToString(ColorOutputFormats.RGBA));
        Assert.Equal("255,128,64", color.ToString(ColorOutputFormats.ColorElements));
    }
}