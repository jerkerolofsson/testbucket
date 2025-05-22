using Microsoft.IdentityModel.Tokens;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings.Fakes;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Contains unit tests for access token generation and validation using <see cref="ProjectTokenGenerator"/> and <see cref="AccessTokenValidator"/>.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class AccessTokenTests
    {
        /// <summary>
        /// Verifies that <see cref="ProjectTokenGenerator.GenerateCiCdAccessTokenAsync"/> creates a valid access token that can be validated by <see cref="AccessTokenValidator"/>.
        /// </summary>
        [Component("Identity")]
        [Fact]
        public async Task GenerateCiCdAccessTokenAsync_CreatesValidToken()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            var generator = new ProjectTokenGenerator(settingsProvider);
            var token = await generator.GenerateCiCdAccessTokenAsync("tenant-132", 4, 5, 6);

            var _ = AccessTokenValidator.ValidateToken(settings, token);
        }

        /// <summary>
        /// Verifies that the generated access token contains the correct project ID claim.
        /// </summary>
        [Component("Identity")]
        [Fact]
        public async Task GenerateCiCdAccessTokenAsync_HasProjectId()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            var generator = new ProjectTokenGenerator(settingsProvider);
            var token = await generator.GenerateCiCdAccessTokenAsync("tenant-132", 4, 5, 6);

            var principal = AccessTokenValidator.ValidateToken(settings, token);
            new PrincipalValidator(principal).ThrowIfNoProjectId();
        }

        /// <summary>
        /// Verifies that <see cref="AccessTokenValidator.ValidateToken"/> throws a <see cref="SecurityTokenMalformedException"/> when an invalid token is provided.
        /// </summary>
        [Component("Identity")]
        [Fact]
        public async Task ValidateToken_WithInvalidToken_ThrowsSecurityTokenMalformedException()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();

            Assert.Throws<SecurityTokenMalformedException>(() =>
            {
                AccessTokenValidator.ValidateToken(settings, "123");
            });
        }
    }
}