using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Contains unit tests for the <see cref="PrincipalValidator"/> class, focusing on project ID validation logic.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Component("Identity")]
    [FunctionalTest]
    public class PrincipalValidatorTests
    {
        /// <summary>
        /// Verifies that <see cref="PrincipalValidator.ThrowIfNoProjectId"/> does not throw an exception
        /// when the principal contains a valid project ID.
        /// </summary>
        [Fact]
        public void ThrowIfNoProjectId_WithProjectId_DoesNotThrowException()
        {
            var principal = Impersonation.Impersonate("1234", 1);
            var validator = new PrincipalValidator(principal);
            validator.ThrowIfNoProjectId();
        }

        /// <summary>
        /// Verifies that <see cref="PrincipalValidator.ThrowIfNoProjectId"/> throws an <see cref="UnauthorizedAccessException"/>
        /// when the principal does not contain a project ID.
        /// </summary>
        [Fact]
        public void ThrowIfNoProjectId_WithoutProjectId_ThrowsUnauthorizedAccessException()
        {
            var principal = Impersonation.Impersonate("1234");
            var validator = new PrincipalValidator(principal);
            Assert.Throws<UnauthorizedAccessException>(() => validator.ThrowIfNoProjectId());
        }
    }
}