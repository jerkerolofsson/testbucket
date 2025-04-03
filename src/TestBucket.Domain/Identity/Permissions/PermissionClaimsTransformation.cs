using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;

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

            var permissions = await _userPermissionsManager.GetTenantRolePermissionsAsync(principal);

            // Go through the roles in descending order, so we take the permissions from the "higher role" if a user belongs to 
            // multiple roles
            foreach (string role in new string[] { Roles.SUPERADMIN, Roles.ADMIN, Roles.REGULAR_USER, Roles.READ_ONLY })
            {
                foreach (var permission in permissions.Where(x => x.Role == role))
                {
                    if (principal.IsInRole(permission.Role))
                    {
                        var claimType = PermissionClaims.GetClaimTypeFromEntity(permission.Entity);
                        if (claimType is not null)
                        {
                            if (!principal.HasClaim(claim => claim.Type == claimType))
                            {
                                claimsIdentity.AddClaim(new Claim(claimType, ((int)permission.Level).ToString()));
                            }
                        }
                    }
                }
            }
            principal.AddIdentity(claimsIdentity);

            return principal;
        }
    }
}
