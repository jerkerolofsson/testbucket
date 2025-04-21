using System.Security.Claims;

namespace TestBucket.Domain.Identity.Permissions
{
    internal class PermissionClaimGenerator
    {
        /// <summary>
        /// Generates a claim for the specified permissions
        /// </summary>
        /// <param name="principal">The user to generate a claim for</param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        public static Claim Create(ClaimsPrincipal principal, RolePermission[] rolePermissions)
        {
            var roles = rolePermissions.Select(x => x.Role).Distinct().ToArray();

            // Go through the roles in descending order, so we take the permissions from the "higher role" if a user belongs to 
            // multiple roles
            var builder = new EntityPermissionBuilder();
            foreach (string role in roles)
            {
                foreach (var permission in rolePermissions.Where(x => x.Role == role))
                {
                    if (principal.IsInRole(permission.Role))
                    {
                        builder.Add(permission);
                    }
                }
            }

            var userPermissions = builder.Build();
            var serialziedPermissions = PermissionClaimSerializer.Serialize(userPermissions);
            return new Claim(PermissionClaims.Permissions, serialziedPermissions);
        }
    }
}
