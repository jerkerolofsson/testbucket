using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.ApiKeys
{
    public interface IApiKeyAuthenticator
    {
        Task<ClaimsPrincipal?> AuthenticateAsync(string token);
    }
}
