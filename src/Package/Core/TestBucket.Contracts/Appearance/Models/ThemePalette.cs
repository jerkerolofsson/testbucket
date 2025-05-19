using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Appearance.Models;
public class ThemePalette
{
    /// <summary>
    /// Palette colors
    /// </summary>
    public List<ThemeColor> Colors { get; set; } = [];
}
