using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance;

namespace TestBucket.Domain.Appearance.Themes;

internal class PastelLight : DefaultTheme
{
    public override string ToString() => "Pastel Light";

    public PastelLight()
    {
        DarkScheme.Base.Primary = "#f2d5cf";
        LightScheme.Base.Primary = "#dc8a78";
        DarkScheme.Base.Secondary = "#eebebe";
        LightScheme.Base.Secondary = "#dd7878";
        DarkScheme.Base.Tertiary = "#f4b8e4";
        LightScheme.Base.Tertiary = "#ea76cb";
        ChartPalette = DefaultPalettes.Pastel;
    }

}
