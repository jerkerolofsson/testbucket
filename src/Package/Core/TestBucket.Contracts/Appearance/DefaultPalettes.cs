using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Contracts.Appearance;
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
        Name = "Default",
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


    public static readonly ThemePalette Vibrant = new ThemePalette
    {
        Name = "Vibrant",
        Colors = [
            ThemeColor.Parse("#00bf7d"),
            ThemeColor.Parse("#00b4c5"),
            ThemeColor.Parse("#0073e6"),
            ThemeColor.Parse("#2546f0"),
            ThemeColor.Parse("#5928ed"),
        ]
    };


    public static readonly ThemePalette Pastel = new ThemePalette
    {
        Name = "Pastel",
        Colors = [
            ThemeColor.Parse("#dc8a78"),
            ThemeColor.Parse("#ea76cb"),
            ThemeColor.Parse("#8839ef"),
            ThemeColor.Parse("#d20f39"),
            ThemeColor.Parse("#fe640b"),
            ThemeColor.Parse("#40a02b"),
            ThemeColor.Parse("#179299"),
        ]
    };

    public static readonly ThemePalette PastelLight = new ThemePalette
    {
        Name = "Pastel Light",
        Colors = [
            ThemeColor.Parse("#f2d5cf"),
            ThemeColor.Parse("#f4b8e4"),
            ThemeColor.Parse("#ca9ee6"),
            ThemeColor.Parse("#e78284"),
            ThemeColor.Parse("#ef9f76"),
            ThemeColor.Parse("#e5c890"),
            ThemeColor.Parse("#a6d189"),
        ]
    };

    public static readonly ThemePalette Monochromatic = new ThemePalette
    {
        Name = "Monochromatic",
        Colors = [
            ThemeColor.Parse("#b3c7f7"),
            ThemeColor.Parse("#8babf1"),
            ThemeColor.Parse("#0073e6"),
            ThemeColor.Parse("#0461cf"),
            ThemeColor.Parse("#054fb9"),
        ]
    };
    public static readonly ThemePalette Contrasting1= new ThemePalette
    {
        Name = "Contrasting1",
        Colors = [
            ThemeColor.Parse("#c44601"),
            ThemeColor.Parse("#f57600"),
            ThemeColor.Parse("#8babf1"),
            ThemeColor.Parse("#0073e6"),
            ThemeColor.Parse("#054fb9"),
        ]
    };
    public static readonly ThemePalette Contrasting2 = new ThemePalette
    {
        Name = "Contrasting2",
        Colors = [
            ThemeColor.Parse("#5ba300"),
            ThemeColor.Parse("#89ce00"),
            ThemeColor.Parse("#0073e6"),
            ThemeColor.Parse("#e6308a"),
            ThemeColor.Parse("#b51963"),
        ]
    };


    public static readonly ThemePalette BrightGreen = new ThemePalette
    {
        Name = "Bright Green",
        Colors = [
            ThemeColor.Parse("#ccff33"),
            ThemeColor.Parse("#9ef01a"),
            ThemeColor.Parse("#70e000"),
            ThemeColor.Parse("#38b000"),
            ThemeColor.Parse("#008000"),
            ThemeColor.Parse("#007200"),
            ThemeColor.Parse("#006400"),
            ThemeColor.Parse("#004b23"),
        ]
    };

    public static readonly ThemePalette PurpleSunsetHeatmap = new ThemePalette
    {
        Name = "Purple Sunset (Heatmap)",
        Colors = [
            ThemeColor.Parse("#390099"),
            ThemeColor.Parse("#9e0059"),
            ThemeColor.Parse("#ff0054"),
            ThemeColor.Parse("#ff5400"),
            ThemeColor.Parse("#ffbd00"),
        ]
    };

    public static readonly ThemePalette SunsetHeatmap = new ThemePalette
    {
        Name = "Sunset (Heatmap)",
        Colors = [
            ThemeColor.Parse("#ff7b00"),
            ThemeColor.Parse("#ff8800"),
            ThemeColor.Parse("#ff9500"),
            ThemeColor.Parse("#ffa200"),
            ThemeColor.Parse("#ffaa00"),
            ThemeColor.Parse("#ffb700"),
            ThemeColor.Parse("#ffc300"),
            ThemeColor.Parse("#ffd000"),
            ThemeColor.Parse("#ffdd00"),
            ThemeColor.Parse("#ffea00"),
        ]
    };


    public static readonly ThemePalette Sunset = new ThemePalette
    {
        Name = "Sunset",
        Colors = [
            ThemeColor.Parse("#ffea00"),
            ThemeColor.Parse("#ffdd00"),
            ThemeColor.Parse("#ffd000"),
            ThemeColor.Parse("#ffc300"),
            ThemeColor.Parse("#ffb700"),
            ThemeColor.Parse("#ffaa00"),
            ThemeColor.Parse("#ffa200"),
            ThemeColor.Parse("#ff9500"),
            ThemeColor.Parse("#ff8800"),
            ThemeColor.Parse("#ff7b00"),
        ]
    };

    public static readonly ThemePalette Cyan = new ThemePalette
    {
        Name = "Cyan",
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
        Name = "Palette1",
        Colors = [
            ThemeColor.Parse("#780000"),
            ThemeColor.Parse("#C1121F"),
            ThemeColor.Parse("#FDF0D5"),
            ThemeColor.Parse("#003049"),
            ThemeColor.Parse("#669BBC"),
            ]
    };

    /// <summary>
    /// Looks like office colors
    /// </summary>
    public static readonly ThemePalette Corporate = new ThemePalette
    {
        Name = "Corporate",
        Colors = [
            ThemeColor.Parse("#156082"),
            ThemeColor.Parse("#e97132"),
            ThemeColor.Parse("#196b24"),
            ThemeColor.Parse("#0f9ed5"),
            ThemeColor.Parse("#a02b93"),
            ThemeColor.Parse("#4ea72e"),
            ]
    };


    /// <summary>
    /// Looks like office colors
    /// </summary>
    public static readonly ThemePalette ReportingDefault = new ThemePalette
    {
        Name = "Reporting",
        Colors = [
            ThemeColor.Parse("#EF476F"),
            ThemeColor.Parse("#FFD166"),
            ThemeColor.Parse("#06D6A0"),
            ThemeColor.Parse("#118AB2"),
            ThemeColor.Parse("#073B4C"),
            ThemeColor.Parse("#EF476F").ColorDarken(0.2),
            ThemeColor.Parse("#FFD166").ColorDarken(0.2),
            ThemeColor.Parse("#06D6A0").ColorDarken(0.2),
            ThemeColor.Parse("#118AB2").ColorDarken(0.2),
            ThemeColor.Parse("#073B4C").ColorDarken(0.2),
            ]
    };
}
