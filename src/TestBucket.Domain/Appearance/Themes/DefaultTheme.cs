using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Models;
using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Appearance.Themes;
internal class DefaultTheme : TestBucketTheme
{
    public override string ToString() => "Default";

    public DefaultTheme()
    {
        DarkScheme.Base.Background = "#111116";
        DarkScheme.Base.Surface = "#25252e";
        DarkScheme.Base.DialogSurface = "#20202A";

        DarkScheme.Base.TextPrimary = "#f0f6fc";
        DarkScheme.Base.Dark = "#0d1321";
        DarkScheme.Base.Primary = "#7f85f5";
        DarkScheme.Base.Secondary = "#5b5fc7";
        DarkScheme.Base.Tertiary = "#444791";

        DarkScheme.Input.SearchBackground = "#111116";
        DarkScheme.Input.SearchBackgroundHover = "#000";

        DarkScheme.Field.Background = DarkScheme.Base.Surface;
        DarkScheme.Field.BorderColor = DarkScheme.Base.Surface.ColorLighten(0.2);

        LightScheme.Base.TextPrimary = "#111";
        LightScheme.Base.Background = "#eee";
        LightScheme.Base.Surface = "#fff";
        LightScheme.Base.DialogSurface = "#eee";
        LightScheme.Base.Primary = "#4f52b2";
        LightScheme.Base.Secondary = "#383966";
        LightScheme.Base.Tertiary = "#0c3b5e";
        LightScheme.Input.SearchBackground = "#fff";
        LightScheme.Input.SearchBackgroundHover = "#fff";

        LightScheme.Input.Background = "#fff";
        LightScheme.Field.Background = "#fff";
        LightScheme.Field.BorderColor = "#eee";

    }
}
