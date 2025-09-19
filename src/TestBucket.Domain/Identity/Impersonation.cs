using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Identity
{
    /// <summary>
    /// Helper class for services to create ClaimsPrincipal that impersonates a user
    /// </summary>
    public class Impersonation
    {
        /// <summary>
        /// This is used by testing..
        /// </summary>
        private const string _defaultImpersonationEmail = "admin@admin.com";

        public static ClaimsPrincipal ImpersonateUserWithFullAccess(string? tenantId, string? userName, long? projectId = null)
        {
            userName ??= _defaultImpersonationEmail;
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName),
                new Claim("tenant", tenantId ?? throw new Exception("No tenant"))
            };

            // Permissions
            var builder = new EntityPermissionBuilder();
            builder.AddAllPermissions();

            claims.Add(new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build())));
            if (projectId is not null)
            {
                claims.Add(new Claim("project", projectId.Value.ToString()));
            }

            return new ClaimsPrincipal([new ClaimsIdentity(claims)]);
        }
        public static ClaimsPrincipal Impersonate(string? tenantId, long? projectId = null)
        {
            return ImpersonateUserWithFullAccess(tenantId, _defaultImpersonationEmail, projectId);
        }

        public static ClaimsPrincipal ChangeProject(ClaimsPrincipal user, long projectId)
        {
            // Permissions
            var builder = new EntityPermissionBuilder();

            var claims = new List<Claim>(user.Claims.Where(x=>x.Type != "project"));
            claims.Add(new Claim("project", projectId.ToString()));

            return new ClaimsPrincipal([new ClaimsIdentity(claims, user.Identity?.AuthenticationType)]);
        }

        public static ClaimsPrincipal Impersonate(Action<EntityPermissionBuilder> configure)
        {
            // Permissions
            var builder = new EntityPermissionBuilder();
            configure(builder);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, builder.UserName),
                new Claim(ClaimTypes.Email, builder.Email),
            };

            if (builder.TenantId is not null)
            {
                claims.Add(new Claim("tenant", builder.TenantId));
            }
            if (builder.ProjectId is not null)
            {
                claims.Add(new Claim("project", builder.ProjectId.Value.ToString()));
            }

            claims.Add(new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build())));
            return new ClaimsPrincipal([new ClaimsIdentity(claims)]);
        }
    }
}
