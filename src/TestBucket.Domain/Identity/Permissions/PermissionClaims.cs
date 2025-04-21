
namespace TestBucket.Domain.Identity.Permissions
{
    public static class PermissionClaims
    {
        public const string Permissions = "permissions";
        public static void ThrowIfNoPermission(this ClaimsPrincipal principal, PermissionEntityType entityType, PermissionLevel requiredLevel)
        {
            if (!HasPermission(principal, entityType, requiredLevel))
            {
                throw new UnauthorizedAccessException($"The user {principal.Identity?.Name} does not have the required access ({requiredLevel}) for entities of type {entityType}");
            }
        }

        public static bool HasPermission(this ClaimsPrincipal principal, PermissionEntityType entityType, PermissionLevel requiredLevel)
        {
            // Scan all claims, it is possible that a user has multiple claims of the same type
            // if the user belongs to multiple roles.
            foreach (var level in GetPermissionLevelsFromClaims(principal.Claims, entityType))
            {
                if ((level & requiredLevel) == requiredLevel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Searches the claims for a permission claim type based on the specified entity and returns it
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static PermissionLevel GetPermissionLevelFromClaims(IEnumerable<Claim> claims, PermissionEntityType entityType)
        {
            PermissionLevel mergedLevel = PermissionLevel.None;

            foreach(var level in GetPermissionLevelsFromClaims(claims, entityType))
            {
                mergedLevel |= level;
            }

            return mergedLevel;
        }

        public static IEnumerable<PermissionLevel> GetPermissionLevelsFromClaims(IEnumerable<Claim> claims, PermissionEntityType entityType)
        {
            foreach(var permissionClaim in claims.Where(x => x.Type == PermissionClaims.Permissions))
            {
                var userPermissions = PermissionClaimSerializer.Deserialize(permissionClaim.Value);
                var entityPermission = userPermissions.Permisssions.Where(x => x.EntityType == entityType).FirstOrDefault();
                if(entityPermission is not null)
                {
                    yield return entityPermission.Level;
                }
            }
        }
    }
}
