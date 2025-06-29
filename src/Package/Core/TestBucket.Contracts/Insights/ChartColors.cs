using TestBucket.Contracts.Appearance;
using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Contracts.Insights;
public class ChartColors
{

    /// <summary>
    /// Palette used when there are no specific colors assigned
    /// </summary>
    public ThemePalette Palette { get; set; } = DefaultPalettes.Default;

    /// <summary>
    /// Defines how the color is assigned
    /// </summary>
    public ChartColorMode ColorMode { get; set; } = ChartColorMode.ByLabel;

    /// <summary>
    /// Color of grid lines
    /// </summary>
    public string? GridLineColor { get; set; }

    /// <summary>
    /// Color of tick labels
    /// </summary>
    public string? TickLabelColor { get; set; }

}
