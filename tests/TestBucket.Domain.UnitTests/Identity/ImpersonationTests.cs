using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings.Fakes;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Identity
{
    [UnitTest]
    [EnrichedTest]
    public class ImpersonationTests
    {
        [Component("Identity")]
        [Fact]
        public void ImpersonateTenant_TenantClaimAdded()
        {
            var principal = Impersonation.Impersonate("1234");
            var tenantId = principal.GetTenantId();
            Assert.Equal("1234", tenantId);
        }

        [Component("Identity")]
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
