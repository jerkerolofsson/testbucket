using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Appearance.Models;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Appearance;
public class TestBucketThemeManager : ITestBucketThemeManager
{
    private readonly IUserPreferencesManager _userPreferencesService;

    public TestBucketThemeManager(IUserPreferencesManager userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public async Task<TestBucketBaseTheme> GetThemeByNameAsync(string name)
    {
        // Temporarily simulate a DB delay
        await Task.Delay(50);

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
        if (isDarkMode)
        {
            stylesheet.AppendLine($" --mud-palette-surface: #111;");
            stylesheet.AppendLine($" --mud-palette-background: #000;");
            stylesheet.AppendLine($" --mud-palette-black: #000;");

            stylesheet.AppendLine($" --mud-palette-primary: hotpink;");
        }
        else
        {
            stylesheet.AppendLine($" --mud-palette-surface: #fff;");
            stylesheet.AppendLine($" --mud-palette-background: #eee;");

            stylesheet.AppendLine($" --mud-palette-primary: hotpink;");
        }
    }
}
