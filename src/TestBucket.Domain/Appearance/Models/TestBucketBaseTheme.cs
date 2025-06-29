using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance;
using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Appearance.Models;
public abstract class TestBucketBaseTheme
{
    /// <summary>
    /// Palette used for charts
    /// </summary>
    public ThemePalette ChartPalette { get; set; } = DefaultPalettes.Default;

    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public abstract string Dark { get; }
    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public abstract string Light { get; }
}
