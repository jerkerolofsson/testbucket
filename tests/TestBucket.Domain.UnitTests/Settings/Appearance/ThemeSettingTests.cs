using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Models;
using TestBucket.Domain.Settings.Appearance;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.UnitTests.Settings.Appearance
{
    /// <summary>
    /// Unit tests for the <see cref="ThemeSetting"/> class.
    /// </summary>
    [UnitTest]
    [Component("Settings")]
    [EnrichedTest]
    [FunctionalTest]
    public class ThemeSettingTests
    {
        /// <summary>
        /// Verifies that <see cref="ThemeSetting.ReadAsync"/> returns the user's theme when the principal is valid.
        /// </summary>
        [Fact]
        public async Task ReadAsync_ReturnsTheme_WhenPrincipalIsValid()
        {
            // Arrange
            var theme = "Blue Steel";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user1"),
                new Claim("tenant", "tenant1")
            }, "mock"));

            var preferences = new UserPreferences { TenantId = "tenant1", UserName = "user1", Theme = theme };
            var manager = Substitute.For<IUserPreferencesManager>();
            manager.LoadUserPreferencesAsync(principal).Returns(preferences);

            var setting = new ThemeSetting(manager);
            var context = new SettingContext { Principal = principal };

            // Act
            var result = await setting.ReadAsync(context);

            // Assert
            Assert.Equal(theme, result.StringValue);
        }

        /// <summary>
        /// Verifies that <see cref="ThemeSetting.ReadAsync"/> returns <see cref="FieldValue.Empty"/> when the principal is invalid.
        /// </summary>
        [Fact]
        public async Task ReadAsync_ReturnsEmpty_WhenPrincipalIsInvalid()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity());
            var manager = Substitute.For<IUserPreferencesManager>();
            var setting = new ThemeSetting(manager);
            var context = new SettingContext { Principal = principal };

            // Act
            var result = await setting.ReadAsync(context);

            // Assert
            Assert.Equal(0, result.FieldDefinitionId);
        }

        /// <summary>
        /// Verifies that <see cref="ThemeSetting.WriteAsync"/> updates the user's theme when the principal is valid.
        /// </summary>
        [Fact]
        public async Task WriteAsync_UpdatesTheme_WhenPrincipalIsValid()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user1"),
                new Claim("tenant", "tenant1")
            }, "mock"));

            var preferences = new UserPreferences { TenantId = "tenant1", UserName = "user1", Theme = "Default" };
            var manager = Substitute.For<IUserPreferencesManager>();
            manager.LoadUserPreferencesAsync(principal).Returns(preferences);

            var setting = new ThemeSetting(manager);
            var context = new SettingContext { Principal = principal };
            var value = new FieldValue { StringValue = "Le Trigre", FieldDefinitionId = 1 };

            // Act
            await setting.WriteAsync(context, value);

            // Assert
            Assert.Equal("Le Trigre", preferences.Theme);
            await manager.Received(1).SaveUserPreferencesAsync(principal, preferences);
        }

        /// <summary>
        /// Verifies that <see cref="ThemeSetting.WriteAsync"/> does nothing when the principal is invalid.
        /// </summary>
        [Fact]
        public async Task WriteAsync_DoesNothing_WhenPrincipalIsInvalid()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity());
            var manager = Substitute.For<IUserPreferencesManager>();
            var setting = new ThemeSetting(manager);
            var context = new SettingContext { Principal = principal };
            var value = new FieldValue { StringValue = "Blue Steel", FieldDefinitionId = 1 };

            // Act
            await setting.WriteAsync(context, value);

            // Assert
            await manager.DidNotReceiveWithAnyArgs().SaveUserPreferencesAsync(default!, default!);
        }
    }
}