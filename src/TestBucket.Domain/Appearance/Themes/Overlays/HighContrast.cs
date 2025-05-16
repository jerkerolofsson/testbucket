using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Domain.Appearance.Themes.Overlays;
internal class HighContrast : TestBucketTheme
{
    public HighContrast()
    {
        DarkScheme.Base.Background = "#000";
        DarkScheme.Base.Surface = "#000";
        DarkScheme.Base.Primary = "#1aebff";
        DarkScheme.Base.Secondary = "#1aebff";
        DarkScheme.Base.Tertiary = "#1aebff";

        LightScheme.Base.Background = "#000";
        LightScheme.Base.Surface = "#000";
        LightScheme.Base.Primary = "#1aebff";
        LightScheme.Base.Secondary = "#1aebff";
        LightScheme.Base.Tertiary = "#1aebff";
    }
}
