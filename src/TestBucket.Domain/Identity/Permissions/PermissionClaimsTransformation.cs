using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;

using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.Identity.Permissions
{
    public class PermissionClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserPermissionsManager _userPermissionsManager;

        public PermissionClaimsTransformation(IUserPermissionsManager userPermissionsManager)
        {
            _userPermissionsManager = userPermissionsManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            var rolePermissions = await _userPermissionsManager.GetTenantRolePermissionsAsync(principal);
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
            claimsIdentity.AddClaim(new Claim(PermissionClaims.Permissions, serialziedPermissions));

            principal.AddIdentity(claimsIdentity);

            return principal;
        }
    }
}
