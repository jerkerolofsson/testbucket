using TestBucket.Contracts.Identity;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Fakes;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Tests for The ApiKeyGenerator class
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Component("Identity")]
    [FunctionalTest]
    public class ApiKeyGeneratorTests
    {
        /// <summary>
        /// Generates an access token in scope of 'runner', using a ClaimsPrincipal with a tenant claim and verifies that the tenant claim is copied to the access token
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GenerateAccessToken_WithRunnerScopeAndTenantClaim_TokenHasTenantClaim()
        {
            var settings = await new FakeSettingsProvider().LoadGlobalSettingsAsync();

            var scope = "runner";
            var principal = Impersonation.Impersonate("1234");
            DateTime expires = DateTime.Today.AddDays(10);

            var apiKeyGenerator = new ApiKeyGenerator(settings.SymmetricJwtKey!, settings.JwtIssuer!, settings.JwtAudience!);
            var token = apiKeyGenerator.GenerateAccessToken(scope, principal, expires);

            var principalFromToken = AccessTokenValidator.ValidateToken(settings, token);
            var tenantId = new PrincipalValidator(principal).GetTenantId();
            Assert.NotNull(tenantId);
            Assert.Equal("1234", tenantId);
        }

        /// <summary>
        /// Generates an access token in scope of 'runner', using a ClaimsPrincipal with a project claim and verifies that the project claim is copied to the access token
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GenerateAccessToken_WithRunnerScopeAndProjectClaim_TokenHasProjectId()
        {
            var settings = await new FakeSettingsProvider().LoadGlobalSettingsAsync();

            var scope = "runner";
            var principal = Impersonation.Impersonate("1234", 11);
            DateTime expires = DateTime.Today.AddDays(10);

            var apiKeyGenerator = new ApiKeyGenerator(settings.SymmetricJwtKey!, settings.JwtIssuer!, settings.JwtAudience!);
            var token = apiKeyGenerator.GenerateAccessToken(scope, principal, expires);

            var principalFromToken = AccessTokenValidator.ValidateToken(settings, token);
            var projectId = new PrincipalValidator(principal).GetProjectId();
            Assert.NotNull(projectId);
            Assert.Equal(11, projectId.Value);
        }
    }
}
