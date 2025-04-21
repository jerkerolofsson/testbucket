using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity
{
    /// <summary>
    /// Helper class for services to create ClaimsPrincipal that impersonates a user
    /// </summary>
    public class Impersonation
    {
        public static ClaimsPrincipal Impersonate(string? tenantId, long? projectId = null)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "system"),
                new Claim(ClaimTypes.Email, "admin@admin.com"),
                new Claim("tenant", tenantId ?? throw new Exception("No tenant"))
            };

            // Permissions
            var builder = new EntityPermissionBuilder();
            builder.Add(PermissionEntityType.TestSuite, PermissionLevel.All);
            builder.Add(PermissionEntityType.TestCase, PermissionLevel.All);
            builder.Add(PermissionEntityType.TestCaseRun, PermissionLevel.All);
            builder.Add(PermissionEntityType.TestRun, PermissionLevel.All);
            builder.Add(PermissionEntityType.TestAccount, PermissionLevel.All);
            builder.Add(PermissionEntityType.TestResource, PermissionLevel.All);
            builder.Add(PermissionEntityType.Project, PermissionLevel.All);
            builder.Add(PermissionEntityType.User, PermissionLevel.All);
            builder.Add(PermissionEntityType.Team, PermissionLevel.All);
            builder.Add(PermissionEntityType.Tenant, PermissionLevel.All);
            builder.Add(PermissionEntityType.Requirement, PermissionLevel.All);
            builder.Add(PermissionEntityType.RequirementSpecification, PermissionLevel.All);
            
            claims.Add(new Claim(PermissionClaims.Permissions, PermissionClaimSerializer.Serialize(builder.Build())));
            if (projectId is not null)
            {
                claims.Add(new Claim("project", projectId.Value.ToString()));
            }

            return new ClaimsPrincipal([new ClaimsIdentity(claims)]);
        }
    }
}
