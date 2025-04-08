using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings.Fakes;
using TUnit.Assertions.AssertConditions.Throws;

namespace TestBucket.Domain.UnitTests.NewFolder
{
    [UnitTest]
    public class AccessTokenTests
    {
        [Test]
        public async Task GenerateCiCdAccessTokenAsync_CreatesValidToken()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            var generator = new ProjectTokenGenerator(settingsProvider);
            var token = await generator.GenerateCiCdAccessTokenAsync("tenant-132", 4, 5, 6);

            var _ = AccessTokenValidator.ValidateToken(settings, token);
        }

        [Test]
        public async Task GenerateCiCdAccessTokenAsync_HasProjectId()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            var generator = new ProjectTokenGenerator(settingsProvider);
            var token = await generator.GenerateCiCdAccessTokenAsync("tenant-132", 4, 5, 6);

            var principal = AccessTokenValidator.ValidateToken(settings, token);
            new PrincipalValidator(principal).ThrowIfNoProjectId();
        }

        [Test]
        public async Task ValidateToken_WithInvalidToken_ThrowsException()
        {
            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            await Assert.That(() =>
            {
                AccessTokenValidator.ValidateToken(settings, "123");
            }).ThrowsException();
            
        }
    }
}
