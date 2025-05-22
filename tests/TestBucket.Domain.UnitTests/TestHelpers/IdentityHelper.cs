using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.UnitTests.TestHelpers
{
    internal class IdentityHelper
    {
        public static ClaimsPrincipal ValidPrincipal => new ClaimsPrincipal([new ClaimsIdentity([new Claim("tenant", "tb")])]);
    }
}
