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
        public static ClaimsPrincipal Impersonate(string? tenantId)
        {
            return new ClaimsPrincipal([new ClaimsIdentity([
                new Claim(ClaimTypes.Name, "system"),
                new Claim(ClaimTypes.Email, "admin@admin.com"),
                new Claim("tenant", tenantId ?? throw new Exception("No tenant"))
                ])]);
        }
    }
}
