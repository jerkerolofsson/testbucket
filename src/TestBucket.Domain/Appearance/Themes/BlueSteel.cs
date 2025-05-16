using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Domain.Appearance.Themes;
internal class BlueSteel : TestBucketTheme
{
    public BlueSteel()
    {
        DarkScheme.Base.TextPrimary = "#cfd8dc";
        DarkScheme.Base.Background = "rgb(15, 36, 46)";
        DarkScheme.Base.Surface = "rgb(25, 46, 56)";
        DarkScheme.Base.Dark = "#0d1321";
        DarkScheme.Base.Primary = "#448cdb";
        DarkScheme.Base.Secondary = "#748cab";
        DarkScheme.Base.Tertiary = "#748cab";

        DarkScheme.Field.Background = "rgb(25, 46, 56)";
        DarkScheme.Field.BorderColor = "rgb(45, 86, 96)";

        LightScheme.Base.Primary = "#448cdb";
        LightScheme.Base.Secondary = "#748cab";
        LightScheme.Base.Tertiary = "#748cab";

    }
}
