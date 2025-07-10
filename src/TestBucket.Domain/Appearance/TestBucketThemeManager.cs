using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

using TestBucket.Domain.Appearance.Models;
using TestBucket.Domain.Appearance.Themes;
using TestBucket.Domain.Appearance.Themes.Overlays;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Appearance;
public class TestBucketThemeManager : ITestBucketThemeManager
{
    private readonly IUserPreferencesManager _userPreferencesService;
    private readonly IMemoryCache _memoryCache;

    public TestBucketThemeManager(IUserPreferencesManager userPreferencesService, IMemoryCache memoryCache)
    {
        _userPreferencesService = userPreferencesService;
        _memoryCache = memoryCache;
    }

    public async Task<TestBucketBaseTheme> GetThemeByNameAsync(string? name)
    {
        name ??= "default";
        await Task.Delay(0);

        var key = "theme:" + name;
        return _memoryCache.GetOrCreate<TestBucketBaseTheme>(key, (e) =>
        {
            switch (name)
            {
                case "Dark Moon":
                    return TestBucketTheme.DarkMoon;
                case "Winter":
                    return TestBucketTheme.Winter;
                case "Retro":
                    return TestBucketTheme.Retro;
                case "Material":
                    return TestBucketTheme.Material;

                case "Le Trigre": // This is intentional!
                case "Blue Steel":
                    return TestBucketTheme.BlueSteel;
            }

            return TestBucketTheme.Default;
        }) ?? TestBucketTheme.Default;
    }

    public async Task<TestBucketBaseTheme> GetCurrentThemeAsync(ClaimsPrincipal user)
    {
        var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(user);
        string themeName = userPreferences.Theme ?? "default";
        return await GetThemeByNameAsync(themeName);
    }


    public async Task<string> GetThemedStylesheetAsync(ClaimsPrincipal user)
    {
        var theme = await GetCurrentThemeAsync(user);
        var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(user);
        return GetThemedStylesheet(theme, userPreferences);
    }

    internal static string GetThemedStylesheet(TestBucketBaseTheme theme, Identity.Models.UserPreferences userPreferences)
    {
        bool highContrast = userPreferences.IncreasedContrast;
        bool highFontSize = userPreferences.IncreasedFontSize;
        bool reducedMotion = userPreferences.ReducedMotion ?? false;
        bool isDarkMode = userPreferences.DarkMode;

        var css = new StringBuilder();

        css.AppendLine($"/* {theme.ToString()} */");

        // Add default base style
        AddBaseStyle(css, isDarkMode);

        // Add theme style
        if (isDarkMode)
        {
            css.AppendLine(theme.Dark);
        }
        else
        {
            css.AppendLine(theme.Light);
        }

        // Override with high-contrast settings
        if (userPreferences.IncreasedContrast)
        {
            AppendHighContrast(css, isDarkMode);
        }
        if (userPreferences.IncreasedFontSize)
        {
            AppendIncreasedFontSize(css, isDarkMode);
        }
        if(userPreferences.ReducedMotion == true)
        {
            AppendReducedMotion(css);
        }

        return css.ToString();
    }

    private static void AddBaseStyle(StringBuilder stylesheet, bool isDarkMode)
    {
        stylesheet.AppendLine(":root {");
        if (isDarkMode)
        {
            stylesheet.AppendLine("color-scheme: dark");
        }
        else
        {
            stylesheet.AppendLine("color-scheme: light");
        }
        stylesheet.AppendLine("}");
    }

    private static void AppendHighContrast(StringBuilder stylesheet, bool isDarkMode)
    {
        // High contrast mode overrides some settings
        var theme = new HighContrast();
        stylesheet.AppendLine("/* High contrast */");
        if (isDarkMode)
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.DarkScheme));
        }
        else
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.LightScheme));
        }
    }

    private static void AppendIncreasedFontSize(StringBuilder stylesheet, bool isDarkMode)
    {
        // High contrast mode overrides some settings
        var theme = new LargeTextOverlay();
        stylesheet.AppendLine("/* Large Text Overlay */");
        if (isDarkMode)
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.DarkScheme));
        }
        else
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.LightScheme));
        }
    }


    private static void AppendReducedMotion(StringBuilder stylesheet)
    {
        stylesheet.AppendLine("/* Reduced Motion */");
        stylesheet.AppendLine("""
            html, body {
                --tb-animation-100: 0s;
                --tb-animation-150: 0ms;
                --tb-animation-200: 0ms;
                --tb-animation-300: 0ms;
            }
            * {
                
                -webkit-transition: none !important;
                -moz-transition: none !important;
                -o-transition: none !important;
                transition: none !important;
            }
            """);
    }
}
