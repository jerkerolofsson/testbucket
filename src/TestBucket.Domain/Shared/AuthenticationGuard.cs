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
        public static void ThrowIfNotAdmin(this ClaimsPrincipal principal)
        {
            if (principal.IsInRole("ADMIN"))
            {
                return;
            }
            throw new InvalidOperationException("This resource can only be accessed by administrators");
        }
        public static void ThrowIfNotSuperAdmin(this ClaimsPrincipal principal)
        {
            if (principal.IsInRole("SUPERADMIN"))
            {
                return;
            }
            throw new InvalidOperationException("This resource can only be accessed by administrators");
        }

        public static string? GetTenantId(this ClaimsPrincipal principal)
        {
            var claims = principal.Claims.Where(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value)).ToList();
            if (claims.Count == 0)
            {
                return null;
            }
            return claims.First().Value;
        }

        public static string GetTenantIdOrThrow(this ClaimsPrincipal principal)
        {
            var claims = principal.Claims.Where(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value)).ToList();
            if (claims.Count == 0)
            {
                throw new InvalidOperationException("User is not authenticated / no tenant was found");
            }
            return claims.First().Value;
        }

        public static string GetTenantIdOrThrow(this ClaimsPrincipal principal, Entity entity)
        {
            var tenantId = AuthenticationGuard.GetTenantIdOrThrow(principal);
            if(tenantId != entity.TenantId)
            {
                throw new InvalidOperationException($"Failed to modify entity. The entity belongs to tenant {entity.TenantId} and user belongs to {tenantId}");
            }
            return tenantId;
        }

        public static string ThrowIfEntityTenantIsDifferent(this ClaimsPrincipal principal, Entity entity)
        {
            var tenantId = AuthenticationGuard.GetTenantIdOrThrow(principal);
            if (tenantId != entity.TenantId)
            {
                throw new InvalidOperationException($"Failed to modify entity. The entity belongs to tenant {entity.TenantId} and user belongs to {tenantId}");
            }
            return tenantId;
        }
        public static void ThrowIfEntityTenantIsDifferent(this ClaimsPrincipal principal, string? entityTenantId)
        {
            var tenantId = AuthenticationGuard.GetTenantIdOrThrow(principal);
            if (tenantId != entityTenantId)
            {
                throw new InvalidOperationException($"Failed to modify entity. The entity belongs to tenant {entityTenantId} and user belongs to {tenantId}");
            }
        }
    }
}
