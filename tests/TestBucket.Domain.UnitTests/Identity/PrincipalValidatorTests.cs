using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.UnitTests.Identity
{
    [UnitTest]
    [EnrichedTest]
    public class PrincipalValidatorTests
    {
        [Component("Identity")]
        [Fact]
        public void ThrowIfNoProjectId_WithProjectId_DoesNotThrowException()
        {
            var principal = Impersonation.Impersonate("1234", 1);
            var validator = new PrincipalValidator(principal);
            validator.ThrowIfNoProjectId();
        }

        [Component("Identity")]
        [Fact]
        public void ThrowIfNoProjectId_WithoutProjectId_ThrowsUnauthorizedAccessException()
        {
            var principal = Impersonation.Impersonate("1234");
            var validator = new PrincipalValidator(principal);
            Assert.Throws<UnauthorizedAccessException>(() => validator.ThrowIfNoProjectId());
        }
    }
}
