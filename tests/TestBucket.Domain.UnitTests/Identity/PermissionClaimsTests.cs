using System.Security.Claims;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Unit tests for permission claims and related permission logic.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Component("Identity")]
    [FunctionalTest]
    public class PermissionClaimsTests
    {
        /// <summary>
        /// Creates a permission claim for a given entity type and permission level.
        /// </summary>
        /// <param name="entityType">The type of entity the permission applies to.</param>
        /// <param name="level">The permission level to assign.</param>
        /// <returns>A <see cref="Claim"/> representing the permission.</returns>
        private static Claim CreatePermissionClaim(PermissionEntityType entityType, PermissionLevel level)
        {
            var userPermissions = new UserPermissions(new[] { new EntityPermission(entityType, level) });
            var serialized = PermissionClaimSerializer.Serialize(userPermissions);
            return new Claim(PermissionClaims.Permissions, serialized);
        }

        /// <summary>
        /// Verifies that <c>HasPermission</c> returns true when the user has the required permission.
        /// </summary>
        [Fact]
        public void HasPermission_ReturnsTrue_WhenUserHasRequiredPermission()
        {
            var claim = CreatePermissionClaim(PermissionEntityType.Project, PermissionLevel.ReadWrite);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }, "test"));

            Assert.True(principal.HasPermission(PermissionEntityType.Project, PermissionLevel.Read));
            Assert.True(principal.HasPermission(PermissionEntityType.Project, PermissionLevel.Write));
        }

        /// <summary>
        /// Verifies that <c>HasPermission</c> returns false when the user lacks the required permission.
        /// </summary>
        [Fact]
        public void HasPermission_ReturnsFalse_WhenUserLacksRequiredPermission()
        {
            var claim = CreatePermissionClaim(PermissionEntityType.Project, PermissionLevel.Read);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }, "test"));

            Assert.False(principal.HasPermission(PermissionEntityType.Project, PermissionLevel.Write));
            Assert.False(principal.HasPermission(PermissionEntityType.Project, PermissionLevel.Delete));
        }

        /// <summary>
        /// Verifies that <c>ThrowIfNoPermission</c> throws an <see cref="UnauthorizedAccessException"/> when the user lacks the required permission.
        /// </summary>
        [Fact]
        public void ThrowIfNoPermission_Throws_WhenUserLacksPermission()
        {
            var claim = CreatePermissionClaim(PermissionEntityType.Project, PermissionLevel.Read);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }, "test"));

            Assert.Throws<UnauthorizedAccessException>(() =>
                principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write));
        }

        /// <summary>
        /// Verifies that <c>GetPermissionLevelFromClaims</c> returns a merged permission level from multiple claims.
        /// </summary>
        [Fact]
        public void GetPermissionLevelFromClaims_ReturnsMergedLevel()
        {
            var claims = new List<Claim>
            {
                CreatePermissionClaim(PermissionEntityType.Project, PermissionLevel.Read),
                CreatePermissionClaim(PermissionEntityType.Project, PermissionLevel.Write)
            };

            var merged = PermissionClaims.GetPermissionLevelFromClaims(claims, PermissionEntityType.Project);
            Assert.True((merged & PermissionLevel.Read) == PermissionLevel.Read);
            Assert.True((merged & PermissionLevel.Write) == PermissionLevel.Write);
        }

        /// <summary>
        /// Verifies that <c>GetPermissionLevelsFromClaims</c> returns all permission levels for a given entity type.
        /// </summary>
        /// <param name="entityType">The entity type to check permissions for.</param>
        [InlineData(PermissionEntityType.Project)]
        [InlineData(PermissionEntityType.Heuristic)]
        [InlineData(PermissionEntityType.TestCase)]
        [InlineData(PermissionEntityType.Requirement)]
        [InlineData(PermissionEntityType.RequirementSpecification)]
        [InlineData(PermissionEntityType.Issue)]
        [InlineData(PermissionEntityType.TestResource)]
        [InlineData(PermissionEntityType.TestAccount)]
        [InlineData(PermissionEntityType.TestRun)]
        [InlineData(PermissionEntityType.User)]
        [InlineData(PermissionEntityType.Team)]
        [Theory]
        public void GetPermissionLevelsFromClaims_ReturnsAllLevels(PermissionEntityType entityType)
        {
            var claims = new List<Claim>
            {
                CreatePermissionClaim(entityType, PermissionLevel.Read),
                CreatePermissionClaim(entityType, PermissionLevel.Write)
            };

            var levels = PermissionClaims.GetPermissionLevelsFromClaims(claims, entityType).ToList();
            Assert.Contains(PermissionLevel.Read, levels);
            Assert.Contains(PermissionLevel.Write, levels);
        }
    }
}