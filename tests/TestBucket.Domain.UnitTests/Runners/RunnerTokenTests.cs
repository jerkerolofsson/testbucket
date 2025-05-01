using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Identity;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Settings.Fakes;
using TestBucket.Domain.UnitTests.TestHelpers;

namespace TestBucket.Domain.UnitTests.Runners
{
    [UnitTest]
    [EnrichedTest]
    public class RunnerTokenTests
    {
        [Fact]
        public async Task GenerateAccessToken_WithRunnerScope_HasCorrectScope()
        {
            var principal = Impersonation.Impersonate("abc");

            var settingsProvider = new FakeSettingsProvider();
            var settings = await settingsProvider.LoadGlobalSettingsAsync();
            var generator = new ApiKeyGenerator(settings.SymmetricJwtKey!, settings.JwtIssuer!, settings.JwtAudience!);

            // Act
            var key = generator.GenerateAccessToken("runner", principal, DateTime.UtcNow.AddDays(123));

            // Assert
            var validatedPrincipal = AccessTokenValidator.ValidateToken(settings, key);

            var scopeClaim = validatedPrincipal.Claims.Where(x => x.Type == "scope").FirstOrDefault();
            Assert.NotNull(scopeClaim);
            Assert.Equal("runner", scopeClaim.Value);

            var tenantClaim = validatedPrincipal.Claims.Where(x => x.Type == "tenant").FirstOrDefault();
            Assert.NotNull(tenantClaim);
            Assert.Equal("abc", tenantClaim.Value);
        }
    }
}
