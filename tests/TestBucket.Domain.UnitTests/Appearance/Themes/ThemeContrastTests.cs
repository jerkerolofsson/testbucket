using TestBucket.Domain.Appearance;
using TestBucket.Domain.Appearance.Themes;
using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.UnitTests.Appearance.Themes
{
    /// <summary>
    /// Verifies the contrast ratio of the theme colors against WCAG standards.
    /// </summary>
    [UnitTest]
    [UsabilityTest]
    [EnrichedTest]
    [Feature("Themes")]
    [Component("Appearance")]
    public class ThemeContrastTests
    {
        private const double MinimumContrastRatioExt = 7;
        private const double MinimumContrastRatio = 4.5;

        /// <summary>
        /// Verifies that the contrast ratio for colors for the theme is within the WCAG limits.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("WCAG1.4.6")]
        public void Wcag_WithMaterialTheme_ContrastRatioWithinLimits()
        {
            // Arrange
            var userPreferences = new UserPreferences { TenantId = "abc", UserName = "abc" };
            var theme = new Material();

            // Act
            var css = TestBucketThemeManager.GetThemedStylesheet(theme, userPreferences);

            // Assert
            AssertStylesheetColorsWithinWcagLimits("Material", css);
        }

        /// <summary>
        /// Verifies that the contrast ratio for colors for the theme is within the WCAG limits.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("WCAG1.4.6")]
        public void Wcag_WithDarkMoonTheme_ContrastRatioWithinLimits()
        {
            // Arrange
            var userPreferences = new UserPreferences { TenantId = "abc", UserName = "abc" };
            var theme = new DarkMoon();

            // Act
            var css = TestBucketThemeManager.GetThemedStylesheet(theme, userPreferences);

            // Assert
            AssertStylesheetColorsWithinWcagLimits("DarkMoon", css);
        }

        /// <summary>
        /// Verifies that the contrast ratio for colors for the theme is within the WCAG limits.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("WCAG1.4.6")]
        public void Wcag_WithDefaultLightTheme_ContrastRatioWithinLimits()
        {
            // Arrange
            var userPreferences = new UserPreferences { TenantId = "abc", UserName = "abc", DarkMode = false };
            var theme = new DefaultTheme();

            // Act
            var css = TestBucketThemeManager.GetThemedStylesheet(theme, userPreferences);

            // Assert
            AssertStylesheetColorsWithinWcagLimits("DefaultLight", css);
        }

        /// <summary>
        /// Verifies that the contrast ratio for colors for the theme is within the WCAG limits.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [CoveredRequirement("WCAG1.4.6")]
        public void Wcag_WithDefaultDarkTheme_ContrastRatioWithinLimits()
        {
            // Arrange
            var userPreferences = new UserPreferences { TenantId = "abc", UserName = "abc", DarkMode = true };
            var theme = new DefaultTheme();

            // Act
            var css = TestBucketThemeManager.GetThemedStylesheet(theme, userPreferences);

            // Assert
            AssertStylesheetColorsWithinWcagLimits("DefaultDark", css);
        }

        private void AssertStylesheetColorsWithinWcagLimits(string meterName, string css)
        {
            AssertColorWithinWcagLimits(meterName,"--mud-palette-primary", css);
            AssertColorWithinWcagLimits(meterName, "--mud-palette-secondary", css);
            AssertColorWithinWcagLimits(meterName, "--mud-palette-tertiary", css);

            AssertColorWithinWcagLimits(meterName, "--mud-palette-back", "--mud-palette-text-primary", css);
            AssertColorWithinWcagLimits(meterName, "--mud-palette-surface", "--mud-palette-text-primary", css);
        }

        private void AssertColorWithinWcagLimits(string meterName, string variableName, string css)
        {
            var backgroundVariableName = variableName;
            var textVariableName = variableName + "-text";

            AssertColorWithinWcagLimits(meterName, backgroundVariableName, textVariableName, css);
        }
        private static void AssertColorWithinWcagLimits(string meterName, string backgroundVariableName, string textVariableName, string css)
        {
            var backColor = CssHelper.GetCssVariableColor(backgroundVariableName, css);
            var textColor = CssHelper.GetCssVariableColor(textVariableName, css);

            Assert.NotNull(backColor);
            Assert.NotNull(textColor);

            var contrast = ContrastColorCalculator.CalculateWcagContrastRatio(backColor, textColor);

            var metricName = $"{backgroundVariableName}{textVariableName}";
            TestContext.Current.AddMetric(new Traits.Core.Metrics.TestResultMetric(meterName, metricName, contrast, "ratio"));
            if (contrast >= MinimumContrastRatioExt)
            {
                return;
            }
            if (contrast > MinimumContrastRatio)
            {
                TestContext.Current.AddWarning($"Contrast: {contrast} below limit for WCAG 1.4.6 (Contrast Enhanced) but within limit for WCAG 1.4.3 (Constrast). Background: {backgroundVariableName}, Text: {textVariableName}");
                return;
            }
            Assert.Fail($"Contrast: {contrast} below limit for both WCAG 1.4.3 and WCAG 1.4.6. Background: {backgroundVariableName}, Text: {textVariableName}");
        }
    }
}
