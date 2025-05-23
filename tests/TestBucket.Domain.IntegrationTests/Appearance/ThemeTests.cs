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
    /// <summary>
    /// Verifies the appearance of themes by inspecting the CSS style sheets that are generated when different themes
    /// and settings are used. 
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [EnrichedTest]
    public class ThemeTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that the default theme colors are included in the CSS stylesheet for a user when no theme is set.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetThemeStylesheet_WithNoThemeSet()
        {
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.UserName = Guid.NewGuid().ToString();
                builder.Email = "theme1@admin.com";
            });
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync(user);
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();

            // Act
            var theme = await themeManager.GetThemeByNameAsync(preferences.Theme);
            var stylesheet = await themeManager.GetThemedStylesheetAsync(Fixture.App.SiteAdministrator);

            // Assert
            Assert.NotNull(theme);
            Assert.NotNull(stylesheet);
            Assert.Contains("--mud-palette-primary", stylesheet);
        }

        /// <summary>
        /// Verifies that the CSS stylesheet returned for a user contains the correct colors when an unknown theme is set in the user preferences
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetThemeStylesheet_WithUnknownThemeInDarkMode_StylesheetIsThemedAsDefault()
        {
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.UserName = "theme2" + Guid.NewGuid().ToString();
                builder.Email = "theme2@admin.com";
            });

            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync(user);
            preferences.Theme = "asdfasdf";
            preferences.DarkMode = true;
            await Fixture.UserPreferences.SaveUserPreferencesAsync(preferences, user);

            // Act
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();
            var stylesheet = await themeManager.GetThemedStylesheetAsync(user);

            // Assert
            TestBucketTheme? theme = await themeManager.GetThemeByNameAsync("default") as TestBucketTheme;
            Assert.NotNull(theme?.DarkScheme?.Base?.TextPrimary);

            var expectedColor = theme.DarkScheme.Base.TextPrimary.ToString(Contracts.Appearance.Models.ColorOutputFormats.HexA);
            Assert.NotNull(stylesheet);
            Assert.Contains(expectedColor, stylesheet);
        }

        /// <summary>
        /// Verifies the colors when light mode is used and a theme is set by the user.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("Blue Steel")]
        [InlineData("Default")]
        public async Task GetThemeStylesheet_WithThemeInLightMode_StylesheetIsThemedCorrectly(string themeName)
        {
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.UserName = themeName + Guid.NewGuid().ToString();
                builder.Email = themeName+"theme3@admin.com";
            });
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync(user);
            preferences.Theme = themeName;
            preferences.DarkMode = false;
            await Fixture.UserPreferences.SaveUserPreferencesAsync(preferences, user);

            // Act
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();
            var stylesheet = await themeManager.GetThemedStylesheetAsync(user);

            // Assert
            TestBucketTheme? theme = await themeManager.GetThemeByNameAsync(themeName) as TestBucketTheme;
            Assert.NotNull(theme?.LightScheme?.Base?.TextPrimary);
            Assert.NotNull(stylesheet);
            Assert.Contains(theme.LightScheme.Base.TextPrimary.ToString(Contracts.Appearance.Models.ColorOutputFormats.HexA), stylesheet);
        }

        [Theory]
        [InlineData("Blue Steel")]
        [InlineData("Default")]
        public async Task GetThemeStylesheet_WithThemeInDarkMode_StylesheetIsThemedCorrectly(string themeName)
        {
            var user = Impersonation.Impersonate(builder =>
            {
                builder.TenantId = Fixture.App.Tenant;
                builder.UserName = themeName + "theme4" + Guid.NewGuid().ToString();
                builder.Email = themeName + "theme4@admin.com";
            });
            var preferences = await Fixture.UserPreferences.LoadUserPreferencesAsync(user);
            preferences.Theme = themeName;
            preferences.DarkMode = true;
            await Fixture.UserPreferences.SaveUserPreferencesAsync(preferences, user);

            // Act
            var themeManager = Fixture.Services.GetRequiredService<ITestBucketThemeManager>();
            var stylesheet = await themeManager.GetThemedStylesheetAsync(user);

            // Assert
            TestBucketTheme? theme = await themeManager.GetThemeByNameAsync(themeName) as TestBucketTheme;
            Assert.NotNull(theme?.DarkScheme?.Base?.TextPrimary);
            Assert.NotNull(stylesheet);
            Assert.Contains(theme.DarkScheme.Base.TextPrimary.ToString(Contracts.Appearance.Models.ColorOutputFormats.HexA), stylesheet);
        }

    }
}
