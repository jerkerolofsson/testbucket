using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Contains unit tests for the <see cref="Impersonation"/> class, verifying tenant and project impersonation logic.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Component("Identity")]
    [FunctionalTest]
    public class ImpersonationTests
    {
        /// <summary>
        /// Verifies that impersonating a tenant adds the correct tenant claim to the principal.
        /// </summary>
        [Fact]
        public void ImpersonateTenant_TenantClaimAdded()
        {
            var principal = Impersonation.Impersonate("1234");
            var tenantId = principal.GetTenantId();
            Assert.Equal("1234", tenantId);
        }

        /// <summary>
        /// Verifies that impersonating a tenant and project adds both tenant and project claims to the principal.
        /// </summary>
        [Fact]
        public void ImpersonateTenantAndProject_TenantAndProjectClaimAdded()
        {
            var principal = Impersonation.Impersonate("1234", 1);
            var tenantId = principal.GetTenantId();
            Assert.Equal("1234", tenantId);

            var validator = new PrincipalValidator(principal);
            var projectId = validator.GetProjectId();
            Assert.NotNull(projectId);
            Assert.Equal(1, projectId.Value);
        }
    }
}