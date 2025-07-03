using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Appearance;

/// <summary>
/// Calculates the contrast color
/// </summary>
public class ContrastColorCalculator
{
    /// <summary>
    /// WCAG 1.4.6
    /// </summary>
    private const double MinimumContrastRatio = 7;

    private static readonly ThemeColor _textColorLight = ThemeColor.Parse("#f1f1f1");
    private static readonly ThemeColor _textColorDark = ThemeColor.Parse("#111");

    /// <summary>
    /// Returns a contrasting text color
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public static ThemeColor GetContrastingTextColor(ThemeColor backColor)
    {
        var textColor1 = _textColorLight;
        var textColor2 = _textColorDark;

        return GetConstrastingTextColor(backColor, textColor1, textColor2);
    }

    private static ThemeColor CombineTextColorOnBackgroundWithAlpha(ThemeColor backColor, ThemeColor foreColor)
    {
        var a = foreColor.A / 255.0;
        var modR = (int)Math.Round((1 - a) * backColor.R + a * foreColor.R);
        var modG = (int)Math.Round((1 - a) * backColor.G + a * foreColor.G);
        var modB = (int)Math.Round((1 - a) * backColor.B + a * foreColor.B);
        return new ThemeColor(modR, modG, modB, 1.0);
    }

    public static double CalculateWcagContrastRatio(ThemeColor backColor, ThemeColor textColor)
    {
        /*
         * The WCAG contrast formula is: (L1 + 0.05) / (L2 + 0.05), where L1 is the relative luminance of the lighter color and L2 is the relative luminance of the darker color. 
         * This formula is used to calculate the contrast ratio between two colors, which is a crucial factor in web accessibility
         */

        var fmod = CombineTextColorOnBackgroundWithAlpha(backColor, textColor);
        double l1 = GetLuma(fmod);
        double l2 = GetLuma(backColor);
        double ratio = (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
        return ratio;
    }

    public static double GetSrgb(int rgbComponent)
    {
        double c = rgbComponent / 255.0;
        double srgb = (c <= 0.03928) ? c / 12.92 : Math.Pow(((c + 0.055) / 1.055), 2.4);
        return srgb;
    }

    public static double GetLuma(ThemeColor color)
    {
        return (0.2126 * GetSrgb(color.R) + 0.7152 * GetSrgb(color.G) + 0.0722 * GetSrgb(color.B));
    }

    private static ThemeColor GetConstrastingTextColor(ThemeColor backColor, ThemeColor? textColor1, ThemeColor? textColor2)
    {
        textColor1 ??= _textColorLight;
        textColor2 ??= _textColorDark;

        var contrastRatio1 = CalculateWcagContrastRatio(backColor, textColor1);
        var contrastRatio2 = CalculateWcagContrastRatio(backColor, textColor2);

        if(contrastRatio1 < MinimumContrastRatio && contrastRatio2 < MinimumContrastRatio)
        {
            // Oh no..
        }

        if (contrastRatio1 > contrastRatio2)
        {
            return textColor1;
        }
        return textColor2;
    }
}
