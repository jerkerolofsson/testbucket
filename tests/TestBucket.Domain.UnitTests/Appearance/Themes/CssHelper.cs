using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.UnitTests.Appearance.Themes
{
    internal class CssHelper
    {
        /// <summary>
        /// Helper class to extract CSS variable colors from a stylesheet.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="cascadingStyleSheet"></param>
        /// <returns></returns>
        public static ThemeColor? GetCssVariableColor(string variableName, string cascadingStyleSheet)
        {
            foreach(var line in cascadingStyleSheet.Split('\n'))
            {
                var trimmedLine = line.Trim();
                if(trimmedLine.Contains(variableName))
                {
                    var items = trimmedLine.Split(':');
                    if(items.Length == 2)
                    {
                        ThemeColor color = items[1].TrimEnd(';');
                        return color;
                    }
                }
            }
            return null;
        }
    }
}
