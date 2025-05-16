using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Appearance.Themes;
internal class BlueSteel : DefaultTheme
{
    public BlueSteel()
    {
        DarkScheme.Base.TextPrimary = "#E1F5FE";
        DarkScheme.Base.Background = "#263238";
        DarkScheme.Base.Surface = "#37474F";
        DarkScheme.Base.Dark = "#0d1321";
        DarkScheme.Base.Primary = "#0288D1";
        DarkScheme.Base.Secondary = "#1976D2";
        DarkScheme.Base.Tertiary = "#0097A7";

        DarkScheme.Input.SearchBackground = "#27373F";
        DarkScheme.Input.SearchBackgroundHover = "#17272F";

        DarkScheme.Field.Background = "#37474F";
        DarkScheme.Field.BorderColor = DarkScheme.Field.Background.ColorLighten(0.2).ToString();

        LightScheme.Base.TextPrimary = "#101017";
        LightScheme.Base.Primary = "#0D47A1";
        LightScheme.Base.Secondary = "#01579B";
        LightScheme.Base.Tertiary = "#006064";

    }
}
