using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Appearance;
using TestBucket.Domain.Appearance.Models;
using TestBucket.Domain.Appearance.Themes;

namespace TestBucket.Domain.IntegrationTests.Appearance
{
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    public class ThemeTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        public async Task GetThemeStylesheet_WithNoThemeSet()
        {
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync();
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();

            // Act
            var theme = await themeManager.GetThemeByNameAsync(preferences.Theme);
            var stylesheet = await themeManager.GetThemedStylesheetAsync(Fixture.App.SiteAdministrator);

            // Assert
            Assert.NotNull(theme);
            Assert.NotNull(stylesheet);
            Assert.Contains("--mud-palette-primary", stylesheet);
        }

        [Fact]
        public async Task GetThemeStylesheet_WithUnknownThemeInDarkMode_StylesheetIsThemedAsDefault()
        {
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync();
            preferences.Theme = "asdfasdf";
            preferences.DarkMode = true;
            await Fixture.UserPreferences.SaveUserPreferencesAsync(preferences);

            // Act
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();
            var stylesheet = await themeManager.GetThemedStylesheetAsync(Fixture.App.SiteAdministrator);

            // Assert
            TestBucketTheme? theme = await themeManager.GetThemeByNameAsync("default") as TestBucketTheme;
            Assert.NotNull(theme?.DarkScheme?.Base?.TextPrimary);
            Assert.NotNull(stylesheet);
            Assert.Contains(theme.DarkScheme.Base.TextPrimary.ToString(), stylesheet);
        }

        [Theory]
        [InlineData("Blue Steel")]
        [InlineData("Default")]
        public async Task GetThemeStylesheet_WithThemeInLightMode_StylesheetIsThemedCorrectly(string themeName)
        {
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync();
            preferences.Theme = themeName;
            preferences.DarkMode = false;
            await Fixture.UserPreferences.SaveUserPreferencesAsync(preferences);

            // Act
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();
            var stylesheet = await themeManager.GetThemedStylesheetAsync(Fixture.App.SiteAdministrator);

            // Assert
            TestBucketTheme? theme = await themeManager.GetThemeByNameAsync(themeName) as TestBucketTheme;
            Assert.NotNull(theme?.LightScheme?.Base?.TextPrimary);
            Assert.NotNull(stylesheet);
            Assert.Contains(theme.LightScheme.Base.TextPrimary.ToString(), stylesheet);
        }

        [Theory]
        [InlineData("Blue Steel")]
        [InlineData("Default")]
        public async Task GetThemeStylesheet_WithThemeInDarkMode_StylesheetIsThemedCorrectly(string themeName)
        {
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync();
            preferences.Theme = themeName;
            preferences.DarkMode = true;
            await Fixture.UserPreferences.SaveUserPreferencesAsync(preferences);

            // Act
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();
            var stylesheet = await themeManager.GetThemedStylesheetAsync(Fixture.App.SiteAdministrator);

            // Assert
            TestBucketTheme? theme = await themeManager.GetThemeByNameAsync(themeName) as TestBucketTheme;
            Assert.NotNull(theme?.DarkScheme?.Base?.TextPrimary);
            Assert.NotNull(stylesheet);
            Assert.Contains(theme.DarkScheme.Base.TextPrimary.ToString(), stylesheet);
        }

    }
}
