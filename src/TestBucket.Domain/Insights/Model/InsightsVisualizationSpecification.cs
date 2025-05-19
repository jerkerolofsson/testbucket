using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Insights.Model;
public class InsightsVisualizationSpecification
{
   
    /// <summary>
    /// Data queries
    /// </summary>
    public List<InsightsDataQuery> DataQueries { get; set; } = [];

    /// <summary>
    /// If true, shows the legend
    /// </summary>
    public bool ShowLegend { get; set; }

    /// <summary>
    /// Palette used when there are no specific colors assigned
    /// </summary>
    public ThemePalette? Palette { get; set; }

    /// <summary>
    /// Defines how the color is assigned
    /// </summary>
    public ChartColorMode ColorMode { get; set; } = ChartColorMode.ByLabel;

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
