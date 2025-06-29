namespace TestBucket.Components.Reporting.Models;

[Flags]
public enum ChartType
{
    None = 0,

    /// <summary>
    /// Vertical bars
    /// </summary>
    Bar = 1,

    /// <summary>
    /// Donut
    /// </summary>
    Donut = 2,

    /// <summary>
    /// Pie
    /// </summary>
    Pie = 4,

    /// <summary>
    /// Lines
    /// </summary>
    Line = 8,

    /// <summary>
    /// A timeline heatmap
    /// </summary>
    ActivityHeatmap = 16,

    /// <summary>
    /// Shows one label/value
    /// </summary>
    Text = 32,

    /// <summary>
    /// Stacked bar chart
    /// </summary>
    StackedBar = 64
}
