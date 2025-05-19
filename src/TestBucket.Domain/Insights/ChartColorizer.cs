using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;
using TestBucket.Domain.Appearance;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights;

/// <summary>
/// Assigns colors for a chart
/// </summary>
public class ChartColorizer
{
    private readonly ThemePalette _palette;
    private int _paletteIndex;
    public ChartColorizer(ThemePalette palette)
    {
        _paletteIndex = 0;
        _palette = palette;
    }

    public Dictionary<string,string> GetColorway<U>(InsightsVisualizationSpecification? spec, InsightsData<string,U> data, bool isDarkMode)
    {
        ChartColorMode mode = spec.GetColorMode(isDarkMode);
        var colorway = new Dictionary<string, string>();
        if (mode == ChartColorMode.ByLabel)
        {
            foreach (var series in data.Series)
            {
                foreach (var label in series.Labels)
                {
                    var color = GetColor(spec, label, isDarkMode);
                    colorway[label] = color;
                }
            }
        }
        else if (mode == ChartColorMode.BySeries)
        {
            foreach (var series in data.Series)
            {
                var color = GetColor(spec, series.Name, isDarkMode);
                colorway[series.Name] = color;
            }
        }
        return colorway;
    }

    private string GetColor(InsightsVisualizationSpecification? spec, string label, bool isDarkMode)
    {
        if (spec is null)
        {
            return GetNextPaletteColor(null, isDarkMode);
        }
        else
        {
            var color = spec.GetColor(label);
            if (color is null)
            {
                color = GetNextPaletteColor(spec, isDarkMode);
            }
            return color;
        }
    }

    private string GetNextPaletteColor(InsightsVisualizationSpecification? spec, bool isDarkMode)
    {
        string color;
        var palette = spec?.GetPalette(isDarkMode) ?? _palette;

        var index = _paletteIndex % palette.Colors.Count;
        var paletteColor = palette.Colors[index];
        color = paletteColor.ToString(ColorOutputFormats.HexA);
        _paletteIndex++;
        return color;
    }
}
