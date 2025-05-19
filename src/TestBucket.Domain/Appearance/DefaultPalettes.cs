using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Appearance;
public static class DefaultPalettes
{
    public static Dictionary<string,string> TestResultColors
    {
        get
        {
            return new Dictionary<string, string>
            {
                { "Passed", "rgba(11, 186, 131, 1)" },
                { "Failed", "rgba(246, 78, 98, 1)" },
                { "Blocked", "rgb(250,220,80)" },
                { "Skipped", "rgb(150,150,150)" },
                { "NoRun", "rgb(50,50,50)" }
            };
        }
    }

    public static readonly ThemePalette Default = new ThemePalette
    {
        Colors = [
            ThemeColor.Parse("#7f85f5"),
            ThemeColor.Parse("#5b5fc7"),
            ThemeColor.Parse("#444791"),
            ThemeColor.Parse("#7f85f5").ColorRgbLighten(),
            ThemeColor.Parse("#5b5fc7").ColorRgbLighten(),
            ThemeColor.Parse("#444791").ColorRgbLighten(),
            ThemeColor.Parse("#7f85f5").ColorRgbDarken(),
            ThemeColor.Parse("#5b5fc7").ColorRgbDarken(),
            ThemeColor.Parse("#444791").ColorRgbDarken(),
        ]
    };

    public static readonly ThemePalette Cyan = new ThemePalette
    {
        Colors = [
            ThemeColor.Parse("#07beb8"),
            ThemeColor.Parse("#3dccc7"),
            ThemeColor.Parse("#68d8d6"),
            ThemeColor.Parse("#9ceaef"),
            ThemeColor.Parse("#07beb8").ColorRgbDarken(),
            ThemeColor.Parse("#3dccc7").ColorRgbDarken(),
            ThemeColor.Parse("#68d8d6").ColorRgbDarken(),
            ThemeColor.Parse("#9ceaef").ColorRgbDarken(),
        ]
    };

    public static readonly ThemePalette Palette1 = new ThemePalette
    {
        Colors = [
            ThemeColor.Parse("#780000"),
            ThemeColor.Parse("#C1121F"),
            ThemeColor.Parse("#FDF0D5"),
            ThemeColor.Parse("#003049"),
            ThemeColor.Parse("#669BBC"),
            ]
    };

    /// <summary>
    /// 
    /// </summary>
    public static readonly ThemePalette Corporate = new ThemePalette
    {
        Colors = [
            ThemeColor.Parse("#156082"),
            ThemeColor.Parse("#e97132"),
            ThemeColor.Parse("#196b24"),
            ThemeColor.Parse("#0f9ed5"),
            ThemeColor.Parse("#a02b93"),
            ThemeColor.Parse("#4ea72e"),
            ]
    };
}
