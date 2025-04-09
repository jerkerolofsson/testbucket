using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings.Fakes;


namespace TestBucket.Domain.UnitTests.NewFolder
{
    [UnitTest]
    [EnrichedTest]
    public class AccessTokenTests
    {
        [Fact]
        public async Task GenerateCiCdAccessTokenAsync_CreatesValidToken()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            var generator = new ProjectTokenGenerator(settingsProvider);
            var token = await generator.GenerateCiCdAccessTokenAsync("tenant-132", 4, 5, 6);

            var _ = AccessTokenValidator.ValidateToken(settings, token);
        }

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
