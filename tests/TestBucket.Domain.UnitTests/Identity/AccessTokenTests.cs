using Microsoft.IdentityModel.Tokens;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings.Fakes;


namespace TestBucket.Domain.UnitTests.Identity
{
    [UnitTest]
    [EnrichedTest]
    public class AccessTokenTests
    {
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
