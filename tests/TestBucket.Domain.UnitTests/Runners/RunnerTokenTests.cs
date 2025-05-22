using TestBucket.Contracts.Identity;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Fakes;

namespace TestBucket.Domain.UnitTests.Runners
{
    /// <summary>
    /// Contains unit tests for validating runner access token generation and claims.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class RunnerTokenTests
    {
        /// <summary>
        /// Verifies that <c>ApiKeyGenerator.GenerateAccessToken"</c> with the "runner" scope
        /// produces a token containing the correct "scope" and "tenant" claims.
        /// </summary>
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
