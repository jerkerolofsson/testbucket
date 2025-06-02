using NSubstitute;
using System.Security.Claims;
using TestBucket.Domain.Shared;


namespace TestBucket.Domain.UnitTests.Shared
{
    /// <summary>
    /// Unit tests for <see cref="AuthenticationGuard"/> extension methods.
    /// </summary>
    [UnitTest]
    [SecurityTest]
    [Component("Identity")]
    public class AuthenticationGuardTests
    {
        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.ThrowIfNotAdmin"/> does not throw when the user is in the ADMIN role.
        /// </summary>
        [Fact]
        public void ThrowIfNotAdmin_UserIsAdmin_DoesNotThrow()
        {
            var principal = Substitute.For<ClaimsPrincipal>();
            principal.IsInRole("ADMIN").Returns(true);

            var ex = Record.Exception(() => principal.ThrowIfNotAdmin());
            Assert.Null(ex);
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.ThrowIfNotAdmin"/> throws when the user is not in the ADMIN role.
        /// </summary>
        [Fact]
        public void ThrowIfNotAdmin_UserIsNotAdmin_Throws()
        {
            var principal = Substitute.For<ClaimsPrincipal>();
            principal.IsInRole("ADMIN").Returns(false);

            Assert.Throws<InvalidOperationException>(() => principal.ThrowIfNotAdmin());
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.ThrowIfNotSuperAdmin"/> does not throw when the user is in the SUPERADMIN role.
        /// </summary>
        [Fact]
        public void ThrowIfNotSuperAdmin_UserIsSuperAdmin_DoesNotThrow()
        {
            var principal = Substitute.For<ClaimsPrincipal>();
            principal.IsInRole("SUPERADMIN").Returns(true);

            var ex = Record.Exception(() => principal.ThrowIfNotSuperAdmin());
            Assert.Null(ex);
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.ThrowIfNotSuperAdmin"/> throws when the user is not in the SUPERADMIN role.
        /// </summary>
        [Fact]
        public void ThrowIfNotSuperAdmin_UserIsNotSuperAdmin_Throws()
        {
            var principal = Substitute.For<ClaimsPrincipal>();
            principal.IsInRole("SUPERADMIN").Returns(false);

            Assert.Throws<InvalidOperationException>(() => principal.ThrowIfNotSuperAdmin());
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.GetTenantId"/> returns the tenant id if present.
        /// </summary>
        [Fact]
        public void GetTenantId_TenantClaimExists_ReturnsTenantId()
        {
            var identity = new ClaimsIdentity(new[] { new Claim("tenant", "tenant-123") });
            var principal = new ClaimsPrincipal(identity);

            var tenantId = principal.GetTenantId();

            Assert.Equal("tenant-123", tenantId);
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.GetTenantId"/> returns null if no tenant claim exists.
        /// </summary>
        [Fact]
        public void GetTenantId_NoTenantClaim_ReturnsNull()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            var tenantId = principal.GetTenantId();

            Assert.Null(tenantId);
        }

        /// <summary>
        /// Verifies that AuthenticationGuard.GetTenantIdOrThrow returns the tenant id if present.
        /// </summary>
        [Fact]
        public void GetTenantIdOrThrow_TenantClaimExists_ReturnsTenantId()
        {
            var identity = new ClaimsIdentity(new[] { new Claim("tenant", "tenant-456") });
            var principal = new ClaimsPrincipal(identity);

            var tenantId = principal.GetTenantIdOrThrow();

            Assert.Equal("tenant-456", tenantId);
        }

        /// <summary>
        /// Verifies that AuthenticationGuard.GetTenantIdOrThrow throws if no tenant claim exists.
        /// </summary>
        [Fact]
        public void GetTenantIdOrThrow_NoTenantClaim_Throws()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            Assert.Throws<UnauthorizedAccessException>(() => principal.GetTenantIdOrThrow());
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.GetTenantIdOrThrow(ClaimsPrincipal, Entity)"/> throws if tenant ids do not match.
        /// </summary>
        [Fact]
        public void GetTenantIdOrThrow_WithEntity_TenantMismatch_Throws()
        {
            var identity = new ClaimsIdentity(new[] { new Claim("tenant", "tenant-1") });
            var principal = new ClaimsPrincipal(identity);
            var entity = new Entity { TenantId = "tenant-2" };

            Assert.Throws<UnauthorizedAccessException>(() => principal.GetTenantIdOrThrow(entity));
        }

        /// <summary>
        /// Verifies that <see cref="AuthenticationGuard.GetTenantIdOrThrow(ClaimsPrincipal, Entity)"/> returns tenant id if they match.
        /// </summary>
        [Fact]
        public void GetTenantIdOrThrow_WithEntity_TenantMatch_ReturnsTenantId()
        {
            var identity = new ClaimsIdentity(new[] { new Claim("tenant", "tenant-1") });
            var principal = new ClaimsPrincipal(identity);
            var entity = new Entity { TenantId = "tenant-1" };

            var tenantId = principal.GetTenantIdOrThrow(entity);

            Assert.Equal("tenant-1", tenantId);
        }
    }
}
