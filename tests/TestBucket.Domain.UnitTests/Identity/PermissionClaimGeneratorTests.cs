using System.Security.Claims;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Identity
{
    /// <summary>
    /// Contains unit tests for the <see cref="PermissionClaimGenerator"/> class, 
    /// verifying correct claim generation based on user roles and permissions.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Component("Identity")]
    [FunctionalTest]
    public class PermissionClaimGeneratorTests
    {
        /// <summary>
        /// Verifies that when a user is not in any groups, the generated claim contains no permissions.
        /// </summary>
        [Fact]
        public void GenerateClaim_WithUserInNoGroups_ClaimHasNoPermissions()
        {
            RolePermission[] rolePermissions = [new RolePermission() { Role = "ADMIN", Level = PermissionLevel.Read, Entity = PermissionEntityType.TestCase }];
            var user = new ClaimsPrincipal();

            var claim = PermissionClaimGenerator.Create(user, rolePermissions);

            var grantedPermissions = PermissionClaimSerializer.Deserialize(claim.Value);
            foreach (var permission in grantedPermissions.Permisssions)
            {
                Assert.Equal(PermissionLevel.None, permission.Level);
            }
        }

        /// <summary>
        /// Verifies that when a user is in a group, the generated claim contains the correct role permissions,
        /// and no permissions are granted for unrelated entities.
        /// </summary>
        [Fact]
        public void GenerateClaim_WithUserInGroups_ClaimHasRolePermissions()
        {
            RolePermission[] rolePermissions = [new RolePermission() { Role = "ADMIN", Level = PermissionLevel.Read, Entity = PermissionEntityType.TestCase }];
            Claim[] claims = [new Claim(ClaimTypes.Role, "ADMIN")];
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var claim = PermissionClaimGenerator.Create(user, rolePermissions);

            var grantedPermissions = PermissionClaimSerializer.Deserialize(claim.Value);
            foreach (var permission in grantedPermissions.Permisssions.Where(x => x.EntityType == PermissionEntityType.TestCase))
            {
                Assert.Equal(PermissionLevel.Read, permission.Level);
            }

            // Verify they didn't get any other permissions
            foreach (var permission in grantedPermissions.Permisssions.Where(x => x.EntityType != PermissionEntityType.TestCase))
            {
                Assert.Equal(PermissionLevel.None, permission.Level);
            }
        }
    }
}