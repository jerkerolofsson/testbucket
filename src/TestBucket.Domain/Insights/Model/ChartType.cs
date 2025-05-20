namespace TestBucket.Components.Reporting.Models;

[Flags]
public enum ChartType
{
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
}
