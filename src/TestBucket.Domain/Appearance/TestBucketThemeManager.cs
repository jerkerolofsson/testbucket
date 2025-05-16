using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Models;
using TestBucket.Domain.Appearance.Themes;
using TestBucket.Domain.Appearance.Themes.Overlays;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Appearance;
public class TestBucketThemeManager : ITestBucketThemeManager
{
    private readonly IUserPreferencesManager _userPreferencesService;

    public TestBucketThemeManager(IUserPreferencesManager userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public async Task<TestBucketBaseTheme> GetThemeByNameAsync(string? name)
    {
        name ??= "default";
        await Task.Delay(0);

        switch (name)
        {
            case "Blue Steel":
                return TestBucketTheme.BlueSteel;
        }

        return TestBucketTheme.Default;
    }

    public async Task<string> GetThemedStylesheetAsync(ClaimsPrincipal user)
    {
        var userPreferences = await _userPreferencesService.LoadUserPreferencesAsync(user);
        string themeName = userPreferences.Theme ?? "default";
        bool highContrast = userPreferences.IncreasedContrast;
        bool highFontSize = userPreferences.IncreasedFontSize;
        bool isDarkMode = userPreferences.DarkMode;

        var css = new StringBuilder();

        // Add default base style
        AddBaseStyle(css, isDarkMode);

        // Add theme style
        var theme = await GetThemeByNameAsync(themeName);
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

        return css.ToString();
    }

    private void AddBaseStyle(StringBuilder stylesheet, bool isDarkMode)
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

    private void AppendHighContrast(StringBuilder stylesheet, bool isDarkMode)
    {
        // High contrast mode overrides some settings
        var theme = new HighContrast();
        if (isDarkMode)
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.DarkScheme));
        }
        else
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.LightScheme));
        }
    }

    private void AppendIncreasedFontSize(StringBuilder stylesheet, bool isDarkMode)
    {
        // High contrast mode overrides some settings
        var theme = new LargeTextOverlay();
        if (isDarkMode)
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.DarkScheme));
        }
        else
        {
            stylesheet.AppendLine(theme.GenerateStyle(theme.LightScheme));
        }
    }
}
