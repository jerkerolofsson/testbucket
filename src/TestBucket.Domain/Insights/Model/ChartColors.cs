using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;
using TestBucket.Domain.Appearance;

namespace TestBucket.Domain.Insights.Model;
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
