using TestBucket.Components.Reporting.Models;
using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Contracts.Insights;
public class InsightsVisualizationSpecification
{
    /// <summary>
    /// Label ID. This will be translated with IStringLocalizer<InsightStrings>
    /// </summary>
    public required string Name { get; set; }
   
    /// <summary>
    /// Data queries
    /// </summary>
    public List<InsightsDataQuery> DataQueries { get; set; } = [];

    /// <summary>
    /// The default type of chart
    /// </summary>
    public ChartType ChartType { get; set; } = ChartType.Bar;

    /// <summary>
    /// The chart types that the user can change between
    /// </summary>
    public ChartType AllowedChartTypes { get; set; } = ChartType.Bar|ChartType.Donut|ChartType.Line|ChartType.Pie|ChartType.Text;

    /// <summary>
    /// Color scheme for light mode
    /// </summary>
    public ChartColors LightModeColors { get; set; } = new() { GridLineColor = "#ddd", TickLabelColor = "#222" };

    /// <summary>
    /// Color scheme for dark mode
    /// </summary>
    public ChartColors DarkModeColors { get; set; } = new() { GridLineColor = "#444", TickLabelColor = "#ddd" };

    /// <summary>
    /// If true, shows the legend
    /// </summary>
    public bool ShowLegend { get; set; }

    /// <summary>
    /// If true, shows a data table
    /// </summary>
    public bool ShowDataTable { get; set; }

    /// <summary>
    /// Defines how the color is assigned
    /// </summary>
    public ChartColorMode ColorMode { get; set; } = ChartColorMode.ByLabel;

    /// <summary>
    /// Label or series to show for text.
    /// If ChartColorMode is Label, this corresponds to a label
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// If showing text, this is a text format
    /// </summary>
    public string? TextFormat { get; set; }

    public int Rows { get; set; } = 15;
    public int Columns { get; set; } = 10;

    public string? GetTickLabelColor(bool isDarkMode)
    {
        if (isDarkMode) return DarkModeColors.TickLabelColor;
        return LightModeColors.TickLabelColor;
    }
    public string? GetGridLineColor(bool isDarkMode)
    {
        if (isDarkMode) return DarkModeColors.GridLineColor;
        return LightModeColors.GridLineColor;
    }

    public ChartColorMode GetColorMode(bool isDarkMode)
    {
        return ColorMode;
    }

    public ThemePalette GetPalette(bool isDarkMode)
    {
        if (isDarkMode) return DarkModeColors.Palette;
        return LightModeColors.Palette;
    }

    /// <summary>
    /// Returns a color to use for a label from any series
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public string? GetColor(string label)
    {
        foreach(var query in DataQueries)
        {
            if(query.Colors is not null && query.Colors.TryGetValue(label, out var color))
            {
                return color;
            }
        }

        return null;
    }
}
