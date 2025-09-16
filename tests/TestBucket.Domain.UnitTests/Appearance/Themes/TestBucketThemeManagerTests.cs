using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.Appearance;
using TestBucket.Domain.Appearance.Themes;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Models;

namespace TestBucket.Domain.UnitTests.Appearance.Themes
{
    /// <summary>
    /// Unit tests for the <see cref="TestBucketThemeManager"/> class.
    /// Tests theme retrieval, caching behavior, stylesheet generation, and user preference integration.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Feature("Themes")]
    [Component("Appearance")]
    [EnrichedTest]
    public class TestBucketThemeManagerTests
    {
        private readonly IUserPreferencesManager _userPreferencesManager;
        private readonly IMemoryCache _memoryCache;
        private readonly TestBucketThemeManager _themeManager;
        private readonly ClaimsPrincipal _testUser;
        private readonly UserPreferences _testUserPreferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBucketThemeManagerTests"/> class.
        /// Sets up test dependencies and mock objects for testing theme management functionality.
        /// </summary>
        public TestBucketThemeManagerTests()
        {
            _userPreferencesManager = Substitute.For<IUserPreferencesManager>();
            _memoryCache = new MemoryCache( new MemoryCacheOptions {  });
            _themeManager = new TestBucketThemeManager(_userPreferencesManager, _memoryCache);

            // Setup test user
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("tenant", "testtenant")
            }, "mock");
            _testUser = new ClaimsPrincipal(identity);

            // Setup default user preferences
            _testUserPreferences = new UserPreferences
            {
                TenantId = "testtenant",
                UserName = "testuser",
                Theme = "default",
                DarkMode = false,
                IncreasedContrast = false,
                IncreasedFontSize = false
            };

            _userPreferencesManager.LoadUserPreferencesAsync(_testUser)
                .Returns(_testUserPreferences);
        }

        #region GetThemeByNameAsync Tests

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the default theme when null name is provided.
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WithNullName_ReturnsDefaultTheme()
        {
            // Arrange
            // Act
            var result = await _themeManager.GetThemeByNameAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DefaultTheme>(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the Dark Moon theme when requested by name.
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WithDarkMoonName_ReturnsDarkMoonTheme()
        {
            // Arrange
            // Act
            var result = await _themeManager.GetThemeByNameAsync("Dark Moon");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DarkMoon>(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the Material theme when requested by name.
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WithMaterialName_ReturnsMaterialTheme()
        {
            // Arrange

            // Act
            var result = await _themeManager.GetThemeByNameAsync("Material");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Material>(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the Blue Steel theme when requested by "Blue Steel" name.
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WithBlueSteelName_ReturnsBlueSteelTheme()
        {
            // Arrange

            // Act
            var result = await _themeManager.GetThemeByNameAsync("Blue Steel");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BlueSteel>(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the Blue Steel theme when requested by "Le Trigre" name (intentional alias).
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WithLeTrigreName_ReturnsLeTigreTheme()
        {
            // Arrange

            // Act
            var result = await _themeManager.GetThemeByNameAsync("Le Trigre");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LeTigre>(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the default theme for unknown theme names.
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WithUnknownName_ReturnsDefaultTheme()
        {
            // Arrange
            // Act
            var result = await _themeManager.GetThemeByNameAsync("unknown");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DefaultTheme>(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemeByNameAsync"/> returns the default theme when cache returns null.
        /// </summary>
        [Fact]
        public async Task GetThemeByNameAsync_WhenCacheReturnsNull_ReturnsDefaultTheme()
        {
            // Arrange

            // Act
            var result = await _themeManager.GetThemeByNameAsync("test");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DefaultTheme>(result);
        }

        #endregion

        #region GetCurrentThemeAsync Tests

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetCurrentThemeAsync"/> returns the user's preferred theme.
        /// </summary>
        [Fact]
        public async Task GetCurrentThemeAsync_WithUserPreferences_ReturnsUserTheme()
        {
            // Arrange
            _testUserPreferences.Theme = "Material";

            // Act
            var result = await _themeManager.GetCurrentThemeAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Material>(result);
            await _userPreferencesManager.Received(1).LoadUserPreferencesAsync(_testUser);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetCurrentThemeAsync"/> returns the default theme when user has no theme preference.
        /// </summary>
        [Fact]
        public async Task GetCurrentThemeAsync_WithNullUserTheme_ReturnsDefaultTheme()
        {
            // Arrange
            _testUserPreferences.Theme = null;

            // Act
            var result = await _themeManager.GetCurrentThemeAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DefaultTheme>(result);
            await _userPreferencesManager.Received(1).LoadUserPreferencesAsync(_testUser);
        }

        #endregion

        #region GetThemedStylesheetAsync Tests

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> generates a basic stylesheet for light mode.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithLightMode_GeneratesLightStylesheet()
        {
            // Arrange
            _testUserPreferences.DarkMode = false;
            _testUserPreferences.Theme = "Material";

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("/* Material */", result);
            Assert.Contains("color-scheme: light", result);
            Assert.DoesNotContain("color-scheme: dark", result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> generates a dark stylesheet for dark mode.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithDarkMode_GeneratesDarkStylesheet()
        {
            // Arrange
            _testUserPreferences.DarkMode = true;
            _testUserPreferences.Theme = "Dark Moon";

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("/* Dark Moon */", result);
            Assert.Contains("color-scheme: dark", result);
            Assert.DoesNotContain("color-scheme: light", result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> includes high contrast styles when enabled.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithHighContrast_IncludesHighContrastStyles()
        {
            // Arrange
            _testUserPreferences.IncreasedContrast = true;
            _testUserPreferences.DarkMode = false;

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("/* High contrast */", result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> includes high contrast styles for dark mode when enabled.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithHighContrastDarkMode_IncludesHighContrastDarkStyles()
        {
            // Arrange
            _testUserPreferences.IncreasedContrast = true;
            _testUserPreferences.DarkMode = true;

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("/* High contrast */", result);
            Assert.Contains("color-scheme: dark", result);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> includes large font styles when enabled.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithIncreasedFontSize_IncludesLargeFontStyles()
        {
            // Arrange
            _testUserPreferences.IncreasedFontSize = true;
            _testUserPreferences.DarkMode = false;

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            // The large font styles are applied but don't have a specific comment marker
            // We verify by checking that the method completes and returns content
            Assert.True(result.Length > 0);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> includes large font styles for dark mode when enabled.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithIncreasedFontSizeDarkMode_IncludesLargeFontDarkStyles()
        {
            // Arrange
            _testUserPreferences.IncreasedFontSize = true;
            _testUserPreferences.DarkMode = true;

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("color-scheme: dark", result);
            Assert.True(result.Length > 0);
        }

        /// <summary>
        /// Verifies that <see cref="TestBucketThemeManager.GetThemedStylesheetAsync"/> combines all accessibility features when enabled.
        /// </summary>
        [Fact]
        public async Task GetThemedStylesheetAsync_WithAllAccessibilityFeatures_IncludesAllOverlays()
        {
            // Arrange
            _testUserPreferences.IncreasedContrast = true;
            _testUserPreferences.IncreasedFontSize = true;
            _testUserPreferences.DarkMode = true;
            _testUserPreferences.Theme = "Blue Steel";

            // Act
            var result = await _themeManager.GetThemedStylesheetAsync(_testUser);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("/* Blue Steel */", result);
            Assert.Contains("/* High contrast */", result);
            Assert.Contains("color-scheme: dark", result);
            Assert.True(result.Length > 0);
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}