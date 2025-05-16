using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Domain.Appearance.Themes.Overlays;
internal class LargeTextOverlay : TestBucketTheme
{
    public LargeTextOverlay()
    {
        TypographySet.Default.Size = "14pt";
        TypographySet.Body1.Size = "14pt";
        TypographySet.Body2.Size = "14pt";
        TypographySet.Subtitle1.Size = "14pt";
        TypographySet.Subtitle2.Size = "14pt";
    }
}
