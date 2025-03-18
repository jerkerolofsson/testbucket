using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Shared
{
    public static class AuthenticationGuard
    {
        public static string GetTentantIdOrThrow(this ClaimsPrincipal principal)
        {
            var claims = principal.Claims.Where(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value)).ToList();
            if(claims.Count == 0)
            {
                throw new InvalidOperationException("User is not authenticated / no tenant was found");
            }
            return claims.First().Value;
        }

        public static string GetTentantIdOrThrow(this ClaimsPrincipal principal, Entity entity)
        {
            var tenantId = AuthenticationGuard.GetTentantIdOrThrow(principal);
            if(tenantId != entity.TenantId)
            {
                throw new InvalidOperationException($"Failed to modify entity. The entity belongs to tenant {entity.TenantId} and user belongs to {tenantId}");
            }
            return tenantId;
        }

        public static void ThrowIfEntityTenantIsDifferent(this ClaimsPrincipal principal, Entity entity)
        {
            var tenantId = AuthenticationGuard.GetTentantIdOrThrow(principal);
            if (tenantId != entity.TenantId)
            {
                throw new InvalidOperationException($"Failed to modify entity. The entity belongs to tenant {entity.TenantId} and user belongs to {tenantId}");
            }
        }
    }
}
